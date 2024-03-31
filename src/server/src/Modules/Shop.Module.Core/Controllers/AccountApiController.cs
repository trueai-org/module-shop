using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using Shop.Module.Schedule;
using System.Web;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 账户API控制器，提供账号、登录、用户信息等功能。
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/account")]
    public class AccountApiController : ControllerBase
    {
        private readonly IRepository<SmsSend> _smsSendRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IRepository<User> _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IWorkContext _workContext;
        private readonly string _webHost;
        private readonly IJobService _jobService;
        private readonly IRepository<Media> _mediaRepository;
        private readonly IAccountService _accountService;

        public AccountApiController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IRepository<SmsSend> smsSendRepository,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IRepository<User> userRepository,
            ITokenService tokenService,
            IWorkContext workContext,
            IJobService jobService,
            IRepository<Media> mediaRepository,
            IAccountService accountService,
            IOptionsMonitor<ShopOptions> config)
        {
            _smsSendRepository = smsSendRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountApiController>();
            _userRepository = userRepository;
            _tokenService = tokenService;
            _workContext = workContext;
            _webHost = config.CurrentValue.WebHost;
            _jobService = jobService;
            _mediaRepository = mediaRepository;
            _accountService = accountService;
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<Result> CurrentUser()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (user == null)
                return Result.Fail("Error");
            var result = new AccountResult()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Culture = user.Culture,
                Email = StringHelper.EmailEncryption(user.Email),
                EmailConfirmed = user.EmailConfirmed,
                FullName = user.FullName,
                LastActivityOn = user.LastActivityOn,
                LastIpAddress = user.LastIpAddress,
                LastLoginOn = user.LastLoginOn,
                PhoneNumber = StringHelper.PhoneEncryption(user.PhoneNumber),
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Avatar = user.AvatarUrl,
                NotifyCount = 20
            };
            return Result.Ok(result);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut()]
        public async Task<Result> PutCurrentUser(UserPutParam param)
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (user == null)
                return Result.Fail("Error");

            if (param.MediaId.HasValue)
            {
                var media = await _mediaRepository.FirstOrDefaultAsync(param.MediaId.Value);
                if (media != null)
                {
                    user.AvatarId = media.Id;
                    user.AvatarUrl = media.Url;
                }
            }

            user.FullName = param.FullName;
            //user.AdminRemark = param.AdminRemark;

            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, false);
            return Result.Ok();
        }

        /// <summary>
        /// 注册验证手机号，并发送注册短信验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register-verify-phone")]
        [AllowAnonymous]
        public async Task<Result> RegisterVerifyPhone(RegisterVerfiyParam model)
        {
            var verify = RegexHelper.VerifyPhone(model.Phone);
            if (!verify.Succeeded)
                return Result.Fail(verify.Message);

            var anyPhone = _userManager.Users.Any(c => c.PhoneNumber == model.Phone);
            if (anyPhone)
                return Result.Fail("此手机号已被注册");

            var code = CodeGen.GenRandomNumber();
            var result = await _smsSender.SendCaptchaAsync(model.Phone, code);
            if (!result.Success)
                return Result.Fail(result.Message);
            return Result.Ok();
        }

        /// <summary>
        /// 注册账号 - 通过手机号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register-by-phone")]
        [AllowAnonymous]
        public async Task<Result> RegisterByPhone(RegisterByPhoneParam model)
        {
            var any = _userManager.Users.Any(c => c.PhoneNumber == model.Phone);
            if (any)
                return Result.Fail("此手机号已被注册");

            //5分钟内的验证码
            //var sms = _smsSendRepository
            //    .Query(c => c.PhoneNumber == model.Phone && c.IsSucceed && !c.IsUsed && c.TemplateType == SmsTemplateType.Captcha
            //    && c.CreatedOn >= DateTime.Now.AddMinutes(-5)).OrderByDescending(c => c.CreatedOn).FirstOrDefault();
            //if (sms == null)
            //    return Result.Fail("验证码不存在或已失效，请重新获取验证码");
            //if (sms.Value != model.Captcha)
            //    return Result.Fail("验证码错误");

            //5分钟内的验证码
            var sms = await _accountService.ValidateGetLastSms(model?.Phone, model?.Captcha);

            var user = new User
            {
                UserName = Guid.NewGuid().ToString("N"),
                FullName = model.Phone,
                PhoneNumber = model.Phone,
                PhoneNumberConfirmed = true,
                IsActive = true,
                Culture = GlobalConfiguration.DefaultCulture
            };
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var verify = RegexHelper.VerifyEmail(model.Email);
                if (!verify.Succeeded)
                    return Result.Fail(verify.Message);
                var anyEmail = _userManager.Users.Any(c => c.Email == model.Email);
                if (anyEmail)
                    return Result.Fail("此邮箱已被使用");
                user.Email = model.Email;
                user.EmailConfirmed = false;
            }

            var transaction = _userRepository.BeginTransaction();
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleWithId.customer.ToString());

                // 通过手机号注册成功的用户不自动登录，因为是JWT认证
                // await _signInManager.SignInAsync(user, isPersistent: false);

                sms.IsUsed = true;
                sms.OutId = user.Id.ToString();
                _smsSendRepository.SaveChanges();
                transaction.Commit();

                if (!string.IsNullOrEmpty(user.Email))
                {
                    // 发送邮箱激活码，激活则绑定邮箱
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await SendEmailConfirmation(user.Email, user.Id, code);
                }
                return Result.Ok();
            }
            return Result.Fail(result.Errors?.Select(c => c.Description).FirstOrDefault());
        }

        /// <summary>
        /// 手机号登录 - 获取手机验证码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login-phone-captcha")]
        [AllowAnonymous]
        public async Task<Result> LoginPhoneGetCaptcha(LoginPhoneGetCaptchaParam model)
        {
            if (!_userManager.Users.Any(c => c.PhoneNumber == model.Phone))
                return Result.Fail("此手机未注册");
            var code = CodeGen.GenRandomNumber();
            var result = await _smsSender.SendCaptchaAsync(model.Phone, code);
            if (!result.Success)
                return Result.Fail(result.Message);
            return Result.Ok();
        }

        /// <summary>
        /// 手机号登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("login-phone")]
        [AllowAnonymous]
        public async Task<Result> LoginPhone(LoginPhoneParam model, string returnUrl = null)
        {
            var phone = model.Phone;

            //5分钟内的验证码
            var sms = await _accountService.ValidateGetLastSms(phone, model?.Code);

            //设置验证码被使用
            sms.IsUsed = true;
            await _smsSendRepository.SaveChangesAsync();

            var user = await _userManager.Users.FirstOrDefaultAsync(c => c.PhoneNumber == phone);
            if (user == null)
                return Result.Fail("用户不存在");
            if (!user.IsActive)
                return Result.Fail("用户已禁用");

            //如果手机没有验证，则自动验证
            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            var isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut)
            {
                throw new Exception("用户已锁定，请稍后重试");
            }

            if (!await _signInManager.CanSignInAsync(user))
            {
                throw new Exception("用户不允许登录，请稍后重试");
            }

            // 如果手机没有验证，则自动验证
            if (!user.PhoneNumberConfirmed)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            // 如果用手机登录且双因子=true时，则设置双因子=false
            if (user.TwoFactorEnabled)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
            }

            // 重置错误次数计数器
            var failedCount = await _userManager.GetAccessFailedCountAsync(user);
            if (failedCount > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

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

            //var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            //if (!userFactors.Any(c => c == nameof(model.Phone)))
            //    return Result.Fail("手机未验证，不允许用手机登录");
            //var isLockedOut = _userManager.IsLockedOutAsync(user);

            //var signInResult = await _shopSignInManager.SignInCheck(user);
            //if (signInResult == null || signInResult.Succeeded)
            //{
            //    //如果返回null，说明被允许登录
            //    //如果用手机登录且双因子=true时，则设置双因子=false
            //    if (user.TwoFactorEnabled)
            //    {
            //        await _userManager.SetTwoFactorEnabledAsync(user, false);
            //    }
            //    var token = await _tokenService.GenerateAccessToken(user);
            //    var loginResult = new LoginResult()
            //    {
            //        Token = token,
            //        Avatar = user.AvatarUrl,
            //        Email = user.Email,
            //        Name = user.FullName,
            //        Phone = user.PhoneNumber
            //    };
            //    return Result.Ok(loginResult);
            //}
            //else if (signInResult.IsLockedOut)
            //{
            //    return Result.Fail("用户已锁定，请稍后重试");
            //}
            //return Result.Fail("用户登录失败，请稍后重试");
        }

        /// <summary>
        /// 用户名/邮箱/手机号登录
        /// https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.aspnetcore.identity.signinresult.isnotallowed?view=aspnetcore-2.2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<Result> Login(LoginParam model, bool includeRefreshToken)
        {
            User user = null;
            if (RegexHelper.VerifyPhone(model.Name).Succeeded)
            {
                // 手机号验证
                user = await _userManager.Users.FirstOrDefaultAsync(c => c.PhoneNumber == model.Name);
                if (!user.PhoneNumberConfirmed)
                    return Result.Fail("手机未验证，不允许用手机登录");
            }
            else if (RegexHelper.VerifyEmail(model.Name).Succeeded)
            {
                // 邮箱验证
                user = await _userManager.FindByEmailAsync(model.Name);
                if (!user.EmailConfirmed)
                    return Result.Fail("邮箱未验证，不允许用邮箱登录");
            }
            else
            {
                // 用户名登录验证
                user = await _userManager.FindByNameAsync(model.Name);
            }
            if (user == null)
                return Result.Fail("用户不存在");
            if (!user.IsActive)
                return Result.Fail("用户已禁用");

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            // update false -> set lockoutOnFailure: true
            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                return Result.Fail("用户已锁定，请稍后重试");
            }
            else if (signInResult.IsNotAllowed)
            {
                return Result.Fail("用户邮箱未验证或手机未验证，不允许登录");
            }
            else if (signInResult.RequiresTwoFactor)
            {
                // 当手机或邮箱验证通过时，TwoFactorEnabled才会生效，否则，则RequiresTwoFactor不可能=true。
                // 设置启用双因素身份验证。
                // 用户帐户已启用双因素身份验证，因此您必须提供第二个身份验证因素。
                var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
                //return Result.Fail(userFactors.Select(c => c), "当前账号存在安全风险，请进一步验证");
                List<(string, string)> ls = new List<(string, string)>();
                foreach (var item in userFactors)
                {
                    if (item == "Phone")
                    {
                        ls.Add((item, StringHelper.PhoneEncryption(user.PhoneNumber)));
                    }
                    else if (item == "Email")
                    {
                        ls.Add((item, StringHelper.EmailEncryption(user.Email)));
                    }
                }
                return Result.Fail(new
                {
                    Providers = ls.Select(c => new { key = c.Item1, value = c.Item2 }),
                    signInResult.RequiresTwoFactor
                }, "当前账号存在安全风险，请进一步验证身份");
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
                if (includeRefreshToken)
                {
                    var refreshToken = _tokenService.GenerateRefreshToken();
                    user.RefreshTokenHash = _userManager.PasswordHasher.HashPassword(user, refreshToken);
                    await _userManager.UpdateAsync(user);
                    loginResult.RefreshToken = refreshToken;
                }
                return Result.Ok(loginResult);
            }
            return Result.Fail("用户登录失败，用户名或密码错误");
        }

        /// <summary>
        /// 登录双因子验证，并发送验证码
        /// two-factor authentication
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login-verify-two-factor")]
        [AllowAnonymous]
        public async Task<Result> LoginVerifyPhone(LoginTwoFactorParam model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Result.Fail("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!userFactors.Any(c => c == model.SelectedProvider))
            {
                return Result.Fail("Error");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (model.SelectedProvider == "Phone")
            {
                var phone = await _userManager.GetPhoneNumberAsync(user);
                var send = await _smsSender.SendCaptchaAsync(phone, code);
                if (!send.Success)
                {
                    return Result.Fail(send.Message);
                }
            }
            else if (model.SelectedProvider == "Email")
            {
                var email = await _userManager.GetEmailAsync(user);
                var message = "Your security code is: " + code;
                await _emailSender.SendEmailAsync(email, "Security Code", message);
            }
            else
            {
                return Result.Fail("Error");
            }
            return Result.Ok();
        }

        /// <summary>
        /// 登录双因子验证
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("login-two-factor")]
        [AllowAnonymous]
        public async Task<Result> LoginTwoFactor(LoginTwoFactorParam model, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return Result.Fail("Error");
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.SelectedProvider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.IsLockedOut)
            {
                return Result.Fail("用户已锁定，请稍后重试");
            }
            else if (result.IsNotAllowed)
            {
                return Result.Fail("用户邮箱未验证或手机未验证，不允许登录");
            }
            else if (result.Succeeded)
            {
                // 如果双因子验证通过，则暂时设置false
                // 如果异地登录则设置true
                // 注意：双因子验证通过后，即便设置为true，在不切换用户的前提下，也不会再次进行双因子验证
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                var token = await _tokenService.GenerateAccessToken(user);
                return Result.Ok(new { token, name = user.FullName, phone = user.PhoneNumber, email = user.Email, returnUrl });
            }
            else
            {
                return Result.Fail("验证码错误");
            }
        }

        /// <summary>
        /// 启用双因子验证
        /// </summary>
        /// <returns></returns>
        [HttpPost("enable-two-factor")]
        public async Task<Result> EnableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(1, "User enabled two-factor authentication.");
            }
            return Result.Ok();
        }

        /// <summary>
        /// 禁用双因子验证
        /// </summary>
        /// <returns></returns>
        [HttpPost("disable-two-factor")]
        public async Task<Result> DisableTwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(2, "User disabled two-factor authentication.");
            }
            return Result.Ok();
        }

        /// <summary>
        /// 发送确认邮件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("send-confirm-email")]
        public async Task<Result> SendConfirmEmail()
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            if (currentUser == null)
                throw new Exception("用户信息异常，请重新登录");
            var user = await _userManager.FindByIdAsync(currentUser.Id.ToString());
            if (user == null)
                throw new Exception("用户信息异常，请重新登录");

            if (string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail("邮箱未设置");
            if (user.EmailConfirmed)
                return Result.Fail("邮箱已激活,请勿重复发送邮件");

            // 发送邮箱激活码，激活则绑定邮箱
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmailConfirmation(user.Email, user.Id, code);
            return Result.Ok();
        }

        /// <summary>
        /// 确认邮件 - 激活邮箱
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("confirm-email")]
        [AllowAnonymous]
        public async Task<Result> ConfirmEmail([FromBody] ConfirmEmailParam param)
        {
            var user = await _userManager.FindByIdAsync(param.UserId.ToString());
            if (user == null)
                return Result.Fail("用户不存在或链接已失效");

            if (user.EmailConfirmed)
                return Result.Ok("邮箱已激活");

            var result = await _userManager.ConfirmEmailAsync(user, param.Code);
            if (result.Succeeded)
                return Result.Ok();

            return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("change-password")]
        public async Task<Result> ChangePassword(ChangePasswordParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            if (currentUser == null)
                throw new Exception("用户信息异常，请重新登录");
            var user = await _userManager.FindByIdAsync(currentUser.Id.ToString());
            if (user == null)
                throw new Exception("用户信息异常，请重新登录");
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                // await _signInManager.SignInAsync(user, isPersistent: false);
                await _signInManager.SignOutAsync();
                _tokenService.RemoveUserToken(user.Id);
                return Result.Ok();
            }
            return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 忘记密码 - 获取找回密码的方式(邮箱/手机)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("forgot-password")]
        [AllowAnonymous]
        public async Task<Result<ForgotPasswordGetResult>> ForgotPassword(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("请输入用户名/邮箱/手机号码");
            name = name.Trim();

            User user = null;
            if (RegexHelper.VerifyPhone(name).Succeeded)
            {
                // 手机号验证
                user = await _userManager.Users.FirstOrDefaultAsync(c => c.PhoneNumber == name);
            }
            else if (RegexHelper.VerifyEmail(name).Succeeded)
            {
                // 邮箱验证
                user = await _userManager.FindByEmailAsync(name);
            }
            else
            {
                // 用户名登录验证
                user = await _userManager.FindByNameAsync(name);
            }
            if (user == null)
                throw new Exception("用户不存在，请确认用户名/邮箱/手机号码是否输入错误");
            if (!user.IsActive)
                throw new Exception("用户已禁用");
            if (string.IsNullOrWhiteSpace(user.PhoneNumber) && string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("用户未绑定邮箱和手机，无法进行找回密码，如需帮助请联系人工客服");

            var result = new ForgotPasswordGetResult()
            {
                //UserId = user.Id, //由于UserId有序性，因此不返回UserId，返回UserName
                UserName = user.UserName,
                Email = StringHelper.EmailEncryption(user.Email),
                Phone = StringHelper.PhoneEncryption(user.PhoneNumber)
            };
            return Result.Ok(result);
        }

        /// <summary>
        /// 邮箱找回 - 发送密码重置邮件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("forgot-password-email")]
        [AllowAnonymous]
        public async Task<Result> ForgotPasswordSendEmail([FromBody] ResetPasswordPostParam param)
        {
            var user = await _userManager.FindByNameAsync(param.UserName);
            if (user == null)
                throw new Exception("用户不存在");
            if (!user.IsActive)
                throw new Exception("用户已禁用");
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new Exception("用户未绑定邮箱，无法通过邮箱找回密码");

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_webHost.Trim('/')}/user/reset-password?userName={user.UserName}&email={StringHelper.EmailEncryption(user.Email)}&code={HttpUtility.UrlEncode(code)}";
            await _jobService.Enqueue(() => _emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callbackUrl}'>REST PASSWORD</a>", true));
            return Result.Ok();
        }

        /// <summary>
        /// 邮箱找回 - 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("reset-password-email")]
        [AllowAnonymous]
        public async Task<Result> ResetPasswordByEmail([FromBody] ResetPasswordPutParam param)
        {
            var user = await _userManager.FindByNameAsync(param.UserName);
            if (user == null)
                throw new Exception("用户不存在");
            if (!user.IsActive)
                throw new Exception("用户已禁用");

            var result = await _userManager.ResetPasswordAsync(user, param.Code, param.Password);
            if (result.Succeeded)
            {
                // 通过邮箱找回密码时，如果邮箱未确认，则自动确认
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }
                _tokenService.RemoveUserToken(user.Id);
                return Result.Ok();
            }
            return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 手机找回 - 发送验证码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("forgot-password-phone")]
        [AllowAnonymous]
        public async Task<Result> ForgotPasswordSendPhone([FromBody] ResetPasswordPostParam param)
        {
            var user = await _userManager.FindByNameAsync(param.UserName);
            if (user == null)
                throw new Exception("用户不存在");
            if (!user.IsActive)
                throw new Exception("用户已禁用");
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                throw new Exception("用户未绑定手机，无法通过手机找回密码");

            var code = CodeGen.GenRandomNumber();
            var result = await _smsSender.SendCaptchaAsync(user.PhoneNumber, code);
            if (!result.Success)
                return Result.Fail(result.Message);
            return Result.Ok();
        }

        /// <summary>
        /// 手机找回 - 重置密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("reset-password-phone")]
        [AllowAnonymous]
        public async Task<Result> ResetPasswordByPhone([FromBody] ResetPasswordPutParam param)
        {
            var user = await _userManager.FindByNameAsync(param.UserName);
            if (user == null)
                throw new Exception("用户不存在");
            if (!user.IsActive)
                throw new Exception("用户已禁用");
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                throw new Exception("用户未绑定手机，无法通过手机找回密码");

            //5分钟内的验证码
            var sms = _smsSendRepository
                .Query(c => c.PhoneNumber == user.PhoneNumber && c.IsSucceed && !c.IsUsed && c.TemplateType == SmsTemplateType.Captcha
                && c.CreatedOn >= DateTime.Now.AddMinutes(-5)).OrderByDescending(c => c.CreatedOn).FirstOrDefault();
            if (sms == null)
                return Result.Fail("验证码不存在或已失效，请重新获取验证码");

            if (sms.Value != param.Code)
                return Result.Fail("验证码错误");

            //设置验证码被使用
            sms.IsUsed = true;
            await _smsSendRepository.SaveChangesAsync();

            //重新生成重置密码的令牌
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, param.Password);
            if (result.Succeeded)
            {
                _tokenService.RemoveUserToken(user.Id);
                return Result.Ok();
            }
            return Result.Fail("重置密码失败，验证码错误或链接已失效，请稍后重试");
        }

        /// <summary>
        /// 移除手机绑定
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove-phone")]
        public async Task<Result> RemovePhoneNumber()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return Result.Ok();
                }
            }
            return Result.Fail("解绑失败");
        }

        /// <summary>
        /// 移除邮箱绑定
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove-email")]
        public async Task<Result> RemoveEmail()
        {
            var user = await _workContext.GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.SetEmailAsync(user, null);
                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return Result.Ok();
                }
            }
            return Result.Fail("解绑失败");
        }

        /// <summary>
        /// 添加手机绑定 - 获取验证码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("add-phone-captcha")]
        public async Task<Result> AddPhoneGetCaptcha(AddPhoneGetCaptchaParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var user = await _userManager.FindByNameAsync(currentUser.UserName);
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                return Result.Fail("已绑定手机,无法再次绑定");

            var any = _userManager.Users.Any(c => c.PhoneNumber == param.Phone);
            if (any)
                return Result.Fail("此手机号已被使用");

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, param.Phone);
            var result = await _smsSender.SendCaptchaAsync(param.Phone, code);
            if (!result.Success)
                return Result.Fail(result.Message);
            return Result.Ok();
        }

        /// <summary>
        /// 添加手机绑定
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("add-phone")]
        public async Task<Result> AddPhone(AddPhoneParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var user = await _userManager.FindByNameAsync(currentUser.UserName);
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                return Result.Fail("已绑定手机,无法再次绑定");

            var any = _userManager.Users.Any(c => c.PhoneNumber == param.Phone);
            if (any)
                return Result.Fail("此手机号已被使用");

            var result = await _userManager.ChangePhoneNumberAsync(user, param.Phone, param.Code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Result.Ok();
            }
            return Result.Fail(result?.Errors?.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 添加邮箱绑定 - 发送绑定链接
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("add-email")]
        public async Task<Result> AddEmailSendToken(AddEmailPostParam param)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var user = await _userManager.FindByNameAsync(currentUser.UserName);
            if (!string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail("已绑定邮箱,无法再次绑定");

            var verify = RegexHelper.VerifyEmail(param.Email);
            if (!verify.Succeeded)
                return Result.Fail(verify.Message);
            var anyEmail = _userManager.Users.Any(c => c.Email == param.Email);
            if (anyEmail)
                return Result.Fail("此邮箱已被使用");

            var code = await _userManager.GenerateChangeEmailTokenAsync(user, param.Email);
            var webHost = _webHost.Trim('/');
            var callbackUrl = $"{webHost}/user/add-email?id={user.Id}&email={StringHelper.EmailEncryption(param.Email)}&code={HttpUtility.UrlEncode(code)}";
            await _jobService.Enqueue(() => _emailSender.SendEmailAsync(param.Email, "Confirm your account", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>VERIFY</a>", true));
            return Result.Ok();
        }

        /// <summary>
        /// 添加邮箱绑定
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPut("add-email")]
        [AllowAnonymous]
        public async Task<Result> AddEmail(AddEmailPutParam param)
        {
            //var currentUser = await _workContext.GetCurrentUser();
            var user = await _userManager.FindByIdAsync(param.UserId.ToString());
            if (user == null)
                return Result.Fail("用户不存在");

            if (!string.IsNullOrWhiteSpace(user.Email))
                return Result.Fail("已绑定邮箱,无法再次绑定");

            var verify = RegexHelper.VerifyEmail(param.Email);
            if (!verify.Succeeded)
                return Result.Fail(verify.Message);
            var anyEmail = _userManager.Users.Any(c => c.Email == param.Email);
            if (anyEmail)
                return Result.Fail("此邮箱已被使用");

            var result = await _userManager.ChangeEmailAsync(user, param.Email, param.Code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Result.Ok();
            }
            return Result.Fail(result?.Errors?.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<Result> LogOff()
        {
            var user = await _workContext.GetCurrentUserAsync();
            await _signInManager.SignOutAsync();
            if (user != null)
                _tokenService.RemoveUserToken(user.Id);
            _logger.LogInformation(4, "User logged out.");
            return Result.Ok();
        }

        private async Task SendEmailConfirmation(string email, int userId, string code)
        {
            if (string.IsNullOrEmpty(email))
                return;
            var webHost = _webHost.Trim('/');
            var callbackUrl = $"{webHost}/user/confirm-email?id={userId}&email={StringHelper.EmailEncryption(email)}&code={HttpUtility.UrlEncode(code)}";
            await _jobService.Enqueue(() => _emailSender.SendEmailAsync(email, "Confirm your account", $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>VERIFY</a>", true));
        }
    }
}