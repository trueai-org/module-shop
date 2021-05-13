using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shop.Infrastructure;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.Models.Cache;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Shop.Module.Core.Services
{
    public class TokenService : ITokenService
    {
        private readonly AuthenticationOptions _config;
        private readonly IStaticCacheManager _cacheManager;
        private readonly UserManager<User> _userManager;

        public TokenService(
            IOptionsMonitor<AuthenticationOptions> config,
            IStaticCacheManager cacheManager,
            UserManager<User> userManager)
        {
            _config = config.CurrentValue;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }
        public async Task<string> GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Jwt.Key));
            var minutes = _config.Jwt.AccessTokenDurationInMinutes;
            var utcNow = DateTime.UtcNow;
            var expires = utcNow.AddMinutes(minutes);
            var claims = await BuildClaims(user);
            var jwtToken = new JwtSecurityToken(
                issuer: _config.Jwt.Issuer,
                audience: "Anyone",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            _cacheManager.Set(ShopKeys.UserJwtTokenPrefix + user.Id, new UserTokenCache()
            {
                UserId = user.Id.ToString(),
                Token = token,
                TokenCreatedOnUtc = utcNow,
                TokenUpdatedOnUtc = utcNow,
                TokenExpiresOnUtc = expires
            }, minutes);
            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidIssuer = _config.Jwt.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Jwt.Key)),
                ValidateLifetime = false //in this case, we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !string.Equals(jwtSecurityToken.Header.Alg, SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        public bool ValidateToken(string identityId, string token)
        {
            if (string.IsNullOrWhiteSpace(identityId) || string.IsNullOrWhiteSpace(token))
                return false;

            var utcNow = DateTime.UtcNow;
            var userToken = _cacheManager.Get<UserTokenCache>(ShopKeys.UserJwtTokenPrefix + identityId);
            if (userToken == null)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(userToken.Token))
            {
                _cacheManager.Remove(ShopKeys.UserJwtTokenPrefix + identityId);
                return false;
            }

            var validate = userToken.Token.Equals(token, StringComparison.OrdinalIgnoreCase);
            if (!validate)
            {
                _cacheManager.Remove(ShopKeys.UserJwtTokenPrefix + identityId);
                return false;
            }

            var minutes = _config.Jwt.AccessTokenDurationInMinutes;
            if (userToken.TokenExpiresOnUtc != null && userToken.TokenExpiresOnUtc < utcNow)
            {
                // 过期
                validate = false;
                _cacheManager.Remove(ShopKeys.UserJwtTokenPrefix + identityId);
            }
            else if (minutes > 0 && userToken.TokenExpiresOnUtc == null)
            {
                // 当调整配置时，访问时更新配置（无过期时间->有过期时间）
                userToken.TokenUpdatedOnUtc = utcNow;
                userToken.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);
                _cacheManager.Set(ShopKeys.UserJwtTokenPrefix + userToken.UserId, userToken, minutes);
            }
            else if (userToken.TokenExpiresOnUtc != null && userToken.TokenType != UserTokenType.Disposable && (utcNow - userToken.TokenUpdatedOnUtc).TotalMinutes >= 1)
            {
                // 如果是一次性令牌则不续签

                // 每分钟自动续签
                // 注意：默认jwt令牌不开启过期策略的

                userToken.TokenUpdatedOnUtc = utcNow;
                userToken.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);
                _cacheManager.Set(ShopKeys.UserJwtTokenPrefix + userToken.UserId, userToken, minutes);
            }
            return validate;
        }

        public void RemoveUserToken(int userId)
        {
            _cacheManager.Remove(ShopKeys.UserJwtTokenPrefix + userId);
        }

        public async Task<IList<Claim>> BuildClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                // https://stackoverflow.com/questions/51119926/jwt-authentication-usermanager-getuserasync-returns-null
                // default the value of UserIdClaimType is ClaimTypes.NameIdentifier, i.e. "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
                new Claim(JwtRegisteredClaimNames.NameId,  user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return claims;
        }
    }
}
