using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.WebApi.Controllers
{
    /// <summary>
    /// 模拟 API 控制器，仅用于开发环境和演示站点，用来模拟超管、买家用户、重置样本数据、重置测试账号密码等。
    /// </summary>
    [ApiController]
    [Route("api/mock")]
    public class MockApiController : ControllerBase
    {
        private readonly IOptionsSnapshot<ShopOptions> _options;
        private readonly IRepository<User> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public MockApiController(IOptionsSnapshot<ShopOptions> options, IRepository<User> userRepository, ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _options = options;

            if (_options.Value.ShopEnv == ShopEnv.PRO)
            {
                throw new Exception("正式环境不允许此操作！");
            }

            _userRepository = userRepository;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 模拟超管用户登录，并自动将令牌保存到 cookie 中，用于 swagger 免输入直接调用接口
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        public async Task<Result> MockAdmin()
        {
            var user = _userRepository.Query(c => c.UserName == "admin").First();
            var token = await _tokenService.GenerateAccessToken(user);
            var loginResult = new LoginResult()
            {
                Token = token,
                Avatar = user.AvatarUrl,
                Email = user.Email,
                Name = user.FullName,
                Phone = user.PhoneNumber
            };

            // 处理 Swagger 刷新记住授权状态
            // 设置 Swagger 刷新自动授权
            Response.Cookies.Append("access-token", token, new CookieOptions() { SameSite = SameSiteMode.None, Secure = true });

            return Result.Ok(loginResult);
        }

        /// <summary>
        /// 重置超管用户密码为 123456
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset-admin-password")]
        public async Task<Result> MockResetAdminPassword()
        {
            var adminUser = _userRepository.Query(c => c.UserName == "admin").First();

            var user = await _userManager.FindByIdAsync(adminUser.Id.ToString());
            if (user == null)
                throw new Exception("用户信息异常");

            var identityResult = await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, "123456");
            if (result.Succeeded)
            {
                // await _signInManager.SignInAsync(user, isPersistent: false);

                await _signInManager.SignOutAsync();

                _tokenService.RemoveUserToken(user.Id);

                return Result.Ok();
            }

            return Result.Ok();
        }

        /// <summary>
        /// 模拟普通/买家/方可用户登录，并自动将令牌保存到 cookie 中，用于 swagger 免输入直接调用接口（注意：普通用户无法调用后台接口，会返回 403 无权限）
        /// </summary>
        /// <returns></returns>
        [HttpGet("guest")]
        public async Task<Result> MockGuest()
        {
            var user = _userRepository.Query().Include(x => x.Roles)
                .FirstOrDefault(x => x.Roles.Any(x => x.RoleId == (int)RoleWithId.guest));

            var token = await _tokenService.GenerateAccessToken(user);
            var loginResult = new LoginResult()
            {
                Token = token,
                Avatar = user.AvatarUrl,
                Email = user.Email,
                Name = user.FullName,
                Phone = user.PhoneNumber
            };

            // 处理 Swagger 刷新记住授权状态
            // 设置 Swagger 刷新自动授权
            Response.Cookies.Append("access-token", token, new CookieOptions() { SameSite = SameSiteMode.None, Secure = true });

            return Result.Ok(loginResult);
        }
    }
}