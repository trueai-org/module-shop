using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.Core.Abstractions.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "admin")]
    public class UserApiController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public UserApiController(
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpGet("quick-search")]
        public async Task<Result> QuickSearch(string nameOrPhone, int take = 20)
        {
            if (take < 0 || take > 100)
                take = 20;

            var query = _userRepository.Query();
            if (!string.IsNullOrWhiteSpace(nameOrPhone))
            {
                query = query.Where(x => x.FullName.Contains(nameOrPhone)
                || x.UserName.Contains(nameOrPhone)
                || x.PhoneNumber.Contains(nameOrPhone));
            }

            var users = await query.Take(take).Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                x.PhoneNumber
            }).ToListAsync();
            return Result.Ok(users);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<UserQueryResult>>> List([FromBody]StandardTableParam<UserQueryParam> param)
        {
            var query = _userRepository.Query();
            var search = param.Search;
            if (search != null)
            {
                if (!string.IsNullOrWhiteSpace(search.Name))
                {
                    query = query.Where(c => c.UserName.Contains(search.Name.Trim()) || c.FullName.Contains(search.Name.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(search.Email))
                {
                    query = query.Where(c => c.Email.Contains(search.Email.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(search.PhoneNumber))
                {
                    query = query.Where(c => c.PhoneNumber.Contains(search.PhoneNumber.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(search.Contact))
                {
                    query = query.Where(c => c.Email.Contains(search.Contact.Trim()) || c.PhoneNumber.Contains(search.Contact.Trim()));
                }
                if (search.IsActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == search.IsActive.Value);
                }
                if (search.RoleIds.Count > 0)
                {
                    var roleIds = search.RoleIds.Distinct().ToList();
                    query = query.Where(x => x.Roles.Any(r => roleIds.Contains(r.RoleId)));
                }
            }
            var result = await query.Include(x => x.Roles)
                  .ToStandardTableResult(param, user => new UserQueryResult
                  {
                      Id = user.Id,
                      IsActive = user.IsActive,
                      UserName = user.UserName,
                      AdminRemark = user.AdminRemark,
                      CreatedOn = user.CreatedOn,
                      Email = user.Email,
                      FullName = user.FullName,
                      LastActivityOn = user.LastActivityOn,
                      LastIpAddress = user.LastIpAddress,
                      LastLoginOn = user.LastLoginOn,
                      PhoneNumber = user.PhoneNumber,
                      UpdatedOn = user.UpdatedOn,
                      RoleIds = user.Roles.Select(c => c.RoleId) // .Distinct().OrderBy(c => c).ToList()
                  });
            return Result.Ok(result);
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var user = await _userRepository.Query()
                  .Include(x => x.Roles)
                  .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                throw new Exception("用户不存在");

            var model = new UserQueryResult
            {
                Id = user.Id,
                IsActive = user.IsActive,
                UserName = user.UserName,
                AdminRemark = user.AdminRemark,
                CreatedOn = user.CreatedOn,
                Email = user.Email,
                FullName = user.FullName,
                LastActivityOn = user.LastActivityOn,
                LastIpAddress = user.LastIpAddress,
                LastLoginOn = user.LastLoginOn,
                PhoneNumber = user.PhoneNumber,
                UpdatedOn = user.UpdatedOn,
                RoleIds = user.Roles.Select(c => c.RoleId) // .Distinct().OrderBy(c => c).ToList()
            };
            return Result.Ok(model);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]UserCreateParam model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                throw new Exception("密码不能为空");

            var any = _userRepository.Query().Any(c => c.UserName == model.UserName);
            if (any)
            {
                return Result.Fail("用户名已存在");
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                IsActive = model.IsActive,
                AdminRemark = model.AdminRemark
            };
            var roleIds = model.RoleIds.Distinct();
            var roleArray = (int[])Enum.GetValues(typeof(RoleWithId));
            foreach (var roleId in roleIds)
            {
                if (!roleArray.Contains(roleId))
                {
                    throw new Exception("角色不存在");
                }
                var userRole = new UserRole
                {
                    RoleId = roleId
                };
                user.Roles.Add(userRole);
                userRole.User = user;
            }

            model.Password = model.Password.Trim();
            if (model.Password.Length < 6 || model.Password.Length > 32)
                throw new Exception("密码长度6-32字符");

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.FirstOrDefault()?.Description);
            }
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]UserCreateParam model)
        {
            var user = await _userRepository.Query()
                   .Include(x => x.Roles)
                   .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                throw new Exception("用户不存在");

            user.Email = model.Email;
            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.IsActive = model.IsActive;
            user.UpdatedOn = DateTime.Now;
            user.AdminRemark = model.AdminRemark;

            AddOrDeleteRoles(model, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.FirstOrDefault()?.Description);
            }
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = model.Password.Trim();
                if (model.Password.Length < 6 || model.Password.Length > 32)
                    throw new Exception("密码长度6-32字符");

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                result = await _userManager.ResetPasswordAsync(user, code, model.Password.Trim());
            }
            if (!result.Succeeded)
            {
                return Result.Fail(result.Errors.FirstOrDefault()?.Description);
            }

            _tokenService.RemoveUserToken(user.Id);
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var user = await _userRepository.Query()
                   .Include(x => x.Roles)
                   .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                throw new Exception("用户不存在");

            user.IsDeleted = true;
            user.UpdatedOn = DateTime.Now;
            foreach (var deletedUserRole in user.Roles.ToList())
            {
                deletedUserRole.User = null;
                user.Roles.Remove(deletedUserRole);
            }

            await _userRepository.SaveChangesAsync();
            _tokenService.RemoveUserToken(user.Id);
            return Result.Ok();
        }

        private void AddOrDeleteRoles(UserCreateParam model, User user)
        {
            var roleArray = (int[])Enum.GetValues(typeof(RoleWithId));
            foreach (var roleId in model.RoleIds)
            {
                if (user.Roles.Any(x => x.RoleId == roleId))
                {
                    continue;
                }
                if (!roleArray.Contains(roleId))
                {
                    throw new Exception("角色不存在");
                }
                var userRole = new UserRole
                {
                    RoleId = roleId,
                    User = user
                };
                user.Roles.Add(userRole);
            }
            var deletedUserRoles =
                user.Roles.Where(userRole => !model.RoleIds.Contains(userRole.RoleId))
                    .ToList();
            foreach (var deletedUserRole in deletedUserRoles)
            {
                deletedUserRole.User = null;
                user.Roles.Remove(deletedUserRole);
            }
        }

        [HttpGet("{userId}/addresses")]
        public async Task<Result> UserAddress(int userId, [FromServices]IRepository<UserAddress> userAddressRepository)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return Result.Fail("用户不存在");
            }
            var userAddress = await userAddressRepository
                .Query()
                .Where(x => x.UserId == userId)
                .Select(x => new UserAddressGetResult
                {
                    UserAddressId = x.Id,
                    AddressType = x.AddressType,
                    ContactName = x.Address.ContactName,
                    Phone = x.Address.Phone,
                    AddressLine1 = x.Address.AddressLine1,
                    CityName = x.Address.City,
                    ZipCode = x.Address.ZipCode,
                    StateOrProvinceId = x.Address.StateOrProvinceId,
                    StateOrProvinceName = x.Address.StateOrProvince.Name,
                    CountryId = x.Address.CountryId,
                    CountryName = x.Address.Country.Name,
                    IsCityEnabled = x.Address.Country.IsCityEnabled,
                    IsDistrictEnabled = x.Address.Country.IsDistrictEnabled,
                }).ToListAsync();

            return Result.Ok(new { Addresses = userAddress, user.DefaultShippingAddressId, user.DefaultBillingAddressId });
        }
    }
}



