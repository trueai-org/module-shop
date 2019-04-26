using Shop.Module.Core.Abstractions.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Core.Abstractions.Services
{
    public interface IUserAddressService
    {
        Task<IList<UserAddressShippingResult>> GetList(int? userAddressId = null);
    }
}
