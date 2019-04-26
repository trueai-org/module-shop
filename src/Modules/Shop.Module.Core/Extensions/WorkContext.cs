using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Abstractions.Data;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Extensions;
using Shop.Module.Core.Abstractions.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Extensions
{
    public class WorkContext : IWorkContext
    {
        private const string UserGuidCookiesName = ShopKeys.UserGuidCookiesName;

        private User _currentUser;
        private HttpContext _httpContext;
        private UserManager<User> _userManager;
        private IRepository<User> _userRepository;

        public WorkContext(
            IHttpContextAccessor contextAccessor,
            UserManager<User> userManager,
            IRepository<User> userRepository)
        {
            _userManager = userManager;
            _httpContext = contextAccessor.HttpContext;
            _userRepository = userRepository;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var contextUser = await GetCurrentUserOrNullAsync();
            if (contextUser != null)
                return contextUser;

            var userGuid = Guid.NewGuid();
            _currentUser = new User
            {
                FullName = RoleWithId.guest.ToString(),
                UserGuid = userGuid,
                UserName = userGuid.ToString("N"),
                Culture = GlobalConfiguration.DefaultCulture,
                IsActive = true
            };
            var abc = await _userManager.CreateAsync(_currentUser, ShopKeys.GuestDefaultPassword);
            await _userManager.AddToRoleAsync(_currentUser, RoleWithId.guest.ToString());
            SetUserGuidCookies(_currentUser.UserGuid);
            return _currentUser;
        }

        public async Task<User> GetCurrentOrThrowAsync()
        {
            var contextUser = await GetCurrentUserOrNullAsync();
            if (contextUser == null)
                throw new Exception("请重新登录");
            return _currentUser;
        }

        public async Task<User> GetCurrentUserOrNullAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            var contextUser = _httpContext?.User;
            if (contextUser == null)
            {
                return _currentUser;
            }

            _currentUser = await _userManager.GetUserAsync(contextUser);

            if (_currentUser != null)
            {
                return _currentUser;
            }

            var userGuid = GetUserGuidFromCookies();
            if (userGuid.HasValue)
            {
                _currentUser = _userRepository.Query().Include(x => x.Roles).FirstOrDefault(x => x.UserGuid == userGuid);
            }

            if (_currentUser != null && _currentUser.Roles.Count == 1 && _currentUser.Roles.First().RoleId == (int)RoleWithId.guest)
            {
                return _currentUser;
            }

            return _currentUser;
        }

        private Guid? GetUserGuidFromCookies()
        {
            if (_httpContext.Request.Cookies.ContainsKey(UserGuidCookiesName))
            {
                return Guid.Parse(_httpContext.Request.Cookies[UserGuidCookiesName]);
            }
            return null;
        }

        private void SetUserGuidCookies(Guid userGuid)
        {
            _httpContext.Response.Cookies.Append(UserGuidCookiesName, userGuid.ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddYears(5),
                HttpOnly = true,
                IsEssential = true
            });
        }
    }
}
