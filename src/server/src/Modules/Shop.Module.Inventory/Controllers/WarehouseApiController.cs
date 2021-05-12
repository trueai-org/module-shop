using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Inventory.Entities;
using Shop.Module.Inventory.ViewModels;
using Shop.Module.Inventory.Areas.Inventory.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Inventory.Areas.Inventory.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/warehouses")]
    public class WarehouseApiController : ControllerBase
    {
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<StateOrProvince> _provinceRepository;
        private readonly IRepository<Product> _productRepository;

        public WarehouseApiController(
            IRepository<Warehouse> warehouseRepository,
            IWorkContext workContext,
            IRepository<Address> addressRepository,
            IRepository<StateOrProvince> provinceRepository,
            IRepository<Product> productRepository)
        {
            _warehouseRepository = warehouseRepository;
            _addressRepository = addressRepository;
            _workContext = workContext;
            _provinceRepository = provinceRepository;
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<Result> Get()
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var query = _warehouseRepository.Query();
            var warehouses = await query.Select(x => new
            {
                x.Id,
                x.Name
            }).ToListAsync();
            return Result.Ok(warehouses);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<WarehouseQueryResult>>> DataList([FromBody]StandardTableParam param)
        {
            var query = _warehouseRepository.Query();
            var result = await query.Include(w => w.Address)
                .ToStandardTableResult(param, warehouse => new WarehouseQueryResult
                {
                    Id = warehouse.Id,
                    Name = warehouse.Name,
                    AddressId = warehouse.Address.Id,
                    ContactName = warehouse.Address.ContactName,
                    AddressLine1 = warehouse.Address.AddressLine1,
                    AddressLine2 = warehouse.Address.AddressLine2,
                    Phone = warehouse.Address.Phone,
                    StateOrProvinceId = warehouse.Address.StateOrProvinceId,
                    CountryId = warehouse.Address.CountryId,
                    City = warehouse.Address.City,
                    ZipCode = warehouse.Address.ZipCode,
                    AdminRemark = warehouse.AdminRemark
                });
            return Result.Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<Result> Get(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var warehouse = await _warehouseRepository.Query().Include(w => w.Address).FirstOrDefaultAsync(w => w.Id == id);
            if (warehouse == null)
                throw new Exception("仓库不存在");
            var address = warehouse.Address ?? new Address();
            var result = new WarehouseQueryResult
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                AddressId = address.Id,
                ContactName = address.ContactName,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Phone = address.Phone,
                StateOrProvinceId = address.StateOrProvinceId,
                CountryId = address.CountryId,
                City = address.City,
                ZipCode = address.ZipCode,
                AdminRemark = warehouse.AdminRemark
            };
            return Result.Ok(result);
        }

        [HttpPost]
        public async Task<Result> Post([FromBody]WarehouseCreateParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();

            //验证所选择省市区是否属于国家
            var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
            if (province == null)
                throw new Exception("省市区不存在");
            if (province.CountryId != model.CountryId)
                throw new Exception("所选择省市区不属于当前选择的国家");

            var address = new Address
            {
                ContactName = model.ContactName,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                Phone = model.Phone,
                StateOrProvinceId = model.StateOrProvinceId,
                CountryId = model.CountryId,
                City = model.City,
                ZipCode = model.ZipCode
            };
            var warehouse = new Warehouse
            {
                Name = model.Name,
                Address = address,
                AdminRemark = model.AdminRemark
            };
            _warehouseRepository.Add(warehouse);
            await _warehouseRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<Result> Put(int id, [FromBody]WarehouseCreateParam model)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var warehouse = await _warehouseRepository.Query()
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (warehouse == null)
                throw new Exception("仓库不存在");

            //验证所选择省市区是否属于国家
            var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
            if (province == null)
                throw new Exception("省市区不存在");
            if (province.CountryId != model.CountryId)
                throw new Exception("所选择省市区不属于当前选择的国家");

            warehouse.Name = model.Name;
            warehouse.AdminRemark = model.AdminRemark;
            warehouse.UpdatedOn = DateTime.Now;
            if (warehouse.Address == null)
            {
                warehouse.Address = new Address();
                _addressRepository.Add(warehouse.Address);
            }
            warehouse.Address.ContactName = model.ContactName;
            warehouse.Address.Phone = model.Phone;
            warehouse.Address.ZipCode = model.ZipCode;
            warehouse.Address.StateOrProvinceId = model.StateOrProvinceId;
            warehouse.Address.CountryId = model.CountryId;
            warehouse.Address.City = model.City;
            warehouse.Address.AddressLine1 = model.AddressLine1;
            warehouse.Address.AddressLine2 = model.AddressLine2;
            warehouse.Address.UpdatedOn = DateTime.Now;
            await _warehouseRepository.SaveChangesAsync();
            return Result.Ok();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<Result> Delete(int id)
        {
            var currentUser = await _workContext.GetCurrentUserAsync();
            var warehouse = await _warehouseRepository.Query()
                .Include(x => x.Address)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (warehouse == null)
                return Result.Fail("仓库不存在");

            //var any = _productRepository.Query().Any(c => c.DefaultWarehouseId == id);
            //if (any)
            //    return Result.Fail("仓库已被产品引用，不允许删除");

            warehouse.IsDeleted = true;
            warehouse.UpdatedOn = DateTime.Now;
            if (warehouse.Address != null)
            {
                warehouse.Address.IsDeleted = true;
                warehouse.Address.UpdatedOn = DateTime.Now;
            }
            await _warehouseRepository.SaveChangesAsync();
            return Result.Ok();
        }
    }
}
