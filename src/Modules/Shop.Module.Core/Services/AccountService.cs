using Microsoft.Extensions.Logging;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.Core.Abstractions.ViewModels;
using Shop.Module.Core.Extensions;
using System;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger _logger;
        private readonly ITokenService _tokenService;
        private readonly ShopSignInManager<User> _shopSignInManager;

        public AccountService(
            ILoggerFactory loggerFactory,
            ITokenService tokenService,
            ShopSignInManager<User> shopSignInManager)
        {
            _logger = loggerFactory.CreateLogger<AccountService>();
            _tokenService = tokenService;
            _shopSignInManager = shopSignInManager;
        }

        public async Task<LoginResult> LoginWithSignInCheck(User user)
        {
            var signInResult = await _shopSignInManager.SignInCheck(user);
            if (signInResult.IsLockedOut)
            {
                throw new Exception("用户已锁定，请稍后重试");
            }
            else if (signInResult == null || signInResult.Succeeded)
            {
                var token = await _tokenService.GenerateAccessToken(user);
                var loginResult = new LoginResult()
                {
                    Token = token,
                    Avatar = user.AvatarUrl,
                    Email = user.Email,
                    Name = user.FullName,
                    Phone = user.PhoneNumber
                };
                return loginResult;
            }
            throw new Exception("用户登录失败，请稍后重试");
        }
    }
}
