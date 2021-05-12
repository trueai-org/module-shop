using Shop.Module.Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface IUserAddressService
    {
        Task<IList<UserAddressShippingResult>> GetList(int? userAddressId = null);
    }
}
