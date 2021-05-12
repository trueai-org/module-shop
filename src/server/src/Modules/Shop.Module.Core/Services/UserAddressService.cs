using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IRepository<UserAddress> _userAddressRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateOrProvince> _stateOrProvinceRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IWorkContext _workContext;
        private readonly ICountryService _countryService;

        public UserAddressService(
            IRepository<UserAddress> userAddressRepository,
            IRepository<Country> countryRepository,
            IRepository<StateOrProvince> stateOrProvinceRepository,
            IRepository<User> userRepository,
            IWorkContext workContext,
            ICountryService countryService)
        {
            _userAddressRepository = userAddressRepository;
            _countryRepository = countryRepository;
            _stateOrProvinceRepository = stateOrProvinceRepository;
            _userRepository = userRepository;
            _workContext = workContext;
            _countryService = countryService;
        }
        public async Task<IList<UserAddressShippingResult>> GetList(int? userAddressId = null)
        {
            var user = await _workContext.GetCurrentUserAsync();
            var countryId = (int)CountryWithId.China;
            var provinces = await _countryService.GetProvinceByCache(countryId);
            if (provinces == null || provinces.Count <= 0)
                throw new Exception("省市区数据异常，请联系管理员");

            var query = _userAddressRepository
                .Query()
                .Include(c => c.Address).ThenInclude(c => c.Country)
                .Where(x => x.AddressType == AddressType.Shipping && x.UserId == user.Id);

            if (userAddressId.HasValue && userAddressId.Value > 0)
            {
                query = query.Where(c => c.Id == userAddressId.Value);
            }

            var list = await query.Select(x => new UserAddressShippingResult
            {
                UserAddressId = x.Id,
                AddressId = x.AddressId,
                ContactName = x.Address.ContactName,
                Phone = x.Address.Phone,
                AddressLine1 = x.Address.AddressLine1,
                ZipCode = x.Address.ZipCode,
                CountryId = x.Address.CountryId,
                CountryName = x.Address.Country.Name,
                StateOrProvinceId = x.Address.StateOrProvinceId,
                UpdatedOn = x.Address.UpdatedOn
            }).ToListAsync();

            foreach (var item in list)
            {
                item.IsDefault = item.UserAddressId == user.DefaultShippingAddressId;

                var first = provinces.FirstOrDefault(c => c.Id == item.StateOrProvinceId);
                if (first != null)
                {
                    StateOrProvinceDto pro = null, city = null, district = null;
                    if (first.Level == StateOrProvinceLevel.Default)
                    {
                        pro = first;
                    }
                    else if (first.Level == StateOrProvinceLevel.City)
                    {
                        city = first;
                        pro = provinces.FirstOrDefault(c => c.Id == city?.ParentId);
                    }
                    else if (first.Level == StateOrProvinceLevel.District)
                    {
                        district = first;
                        city = provinces.FirstOrDefault(c => c.Id == district?.ParentId);
                        pro = provinces.FirstOrDefault(c => c.Id == city?.ParentId);
                    }
                    item.StateOrProvinceId = pro?.Id ?? 0;
                    item.StateOrProvinceName = pro?.Name;
                    item.CityId = city?.Id ?? 0;
                    item.CityName = city?.Name;
                    item.DistrictId = district?.Id;
                    item.DistrictName = district?.Name;
                }
            }
            var result = list.OrderByDescending(c => c.IsDefault).ThenByDescending(c => c.UpdatedOn).ToList();
            return result;
        }
    }
}
