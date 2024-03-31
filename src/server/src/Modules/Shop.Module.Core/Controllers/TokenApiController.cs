using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 管理后台令牌服务相关 API
    /// </summary>
    [ApiController]
    [Route("api/token")]
    public class TokenApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public TokenApiController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<Result> RefeshToken(RefreshTokenParam model)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(model.Token);
            if (principal == null)
            {
                return Result.Fail("Invalid token");
            }

            var user = await _userManager.GetUserAsync(principal);
            var verifyRefreshTokenResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.RefreshTokenHash, model.RefreshToken);
            if (verifyRefreshTokenResult == PasswordVerificationResult.Success)
            {
                var token = await _tokenService.GenerateAccessToken(user);
                return Result.Ok(new { token });
            }

            return Result.Fail("Indicates password verification failed.");
        }
    }
}
