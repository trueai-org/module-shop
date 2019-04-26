using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.ViewModels;
using System.Threading.Tasks;

namespace Shop.Module.Core.Abstractions.Services
{
    public interface IAccountService
    {
        Task<LoginResult> LoginWithSignInCheck(User user);
    }
}
