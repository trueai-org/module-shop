using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.MiniProgram.Data;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.Core.MiniProgram.ViewModels;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.MiniProgram.Controllers
{
    /// <summary>
    /// 微信小程序API控制器，用于处理微信小程序相关的请求，如登录。
    /// </summary>
    [ApiController]
    [Route("api/mp")]
    public class MPApiController : ControllerBase
    {
        private const string Code2SessionUrl = "https://api.weixin.qq.com/sns/jscode2session";
        private const string AccessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token";
        private const string WxaCodeUnlimited = "https://api.weixin.qq.com/wxa/getwxacodeunlimit";

        private readonly MiniProgramOptions _option;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<User> _userRepository;
        private readonly ITokenService _tokenService;

        public MPApiController(
            IAppSettingService appSettingService,
            IAccountService accountService,
            IRepository<UserLogin> userLoginRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IRepository<User> userRepository,
            ITokenService tokenService,
            IOptionsMonitor<MiniProgramOptions> options)
        {
            _option = options.CurrentValue;
            _userLoginRepository = userLoginRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        /// <summary>
        /// 处理微信小程序的登录请求。通过微信小程序传来的code与微信服务器通信以获取用户的唯一标识符。
        /// 若用户是首次登录，则会创建一个新的用户账号。登录成功后，会返回包含访问令牌在内的登录信息。
        /// </summary>
        /// <param name="param">包含微信小程序登录所需信息的参数，例如code。</param>
        /// <returns>返回操作结果，如果登录成功，则包含用户的登录信息，如访问令牌和用户信息。</returns>
        [HttpPost("login")]
        public async Task<Result> Login([FromBody] LoginByMpParam param)
        {
            var url = $"{Code2SessionUrl}?appid={_option.AppId}&secret={_option.AppSecret}&js_code={param.Code}&grant_type=authorization_code";
            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(url);
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Fail("登录失败");
            }

            var result = JsonConvert.DeserializeObject<Code2SessionGetResult>(content);
            if (result.ErrCode != 0)
            {
                return Result.Fail(result.ErrMessage);
            }

            User user = null;
            var model = await _userLoginRepository.Query()
                .FirstOrDefaultAsync(c => c.LoginProvider == MiniProgramDefaults.AuthenticationScheme && c.ProviderKey == result.OpenId);
            if (model == null)
            {
                // 创建用户
                var userName = Guid.NewGuid().ToString("N");
                user = new User
                {
                    UserName = userName,
                    FullName = param.NickName ?? userName,
                    AvatarUrl = param.AvatarUrl,
                    IsActive = true,
                    Culture = GlobalConfiguration.DefaultCulture
                };
                var transaction = _userRepository.BeginTransaction();
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    transaction.Rollback();
                    return Result.Fail("创建用户失败");
                }
                await _userManager.AddToRoleAsync(user, RoleWithId.customer.ToString());
                _userLoginRepository.Add(new UserLogin()
                {
                    LoginProvider = MiniProgramDefaults.AuthenticationScheme,
                    ProviderDisplayName = MiniProgramDefaults.DisplayName,
                    UserId = user.Id,
                    ProviderKey = result.OpenId,
                    UnionId = result.UnionId
                });
                await _userLoginRepository.SaveChangesAsync();
                transaction.Commit();
            }
            else
            {
                user = await _userManager.FindByIdAsync(model.UserId.ToString());
            }
            if (user == null)
            {
                return Result.Fail("登录失败，请稍后重试");
            }

            // var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();
            var signInResult = await _signInManager.ExternalLoginSignInAsync(MiniProgramDefaults.AuthenticationScheme, result.OpenId, false);

            if (signInResult.IsLockedOut)
            {
                return Result.Fail("用户已锁定，请稍后重试");
            }
            else if (signInResult.IsNotAllowed)
            {
                return Result.Fail("用户邮箱未验证或手机未验证，不允许登录");
            }
            else if (signInResult.Succeeded)
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
                return Result.Ok(loginResult);
            }
            return Result.Fail("用户登录失败");
        }
    }
}