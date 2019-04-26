using Shop.Module.Core.Abstractions.Entities;
using System.Threading.Tasks;

namespace Shop.Module.Core.Abstractions.Extensions
{
    public interface IWorkContext
    {
        Task<User> GetCurrentUserAsync();

        Task<User> GetCurrentUserOrNullAsync();

        Task<User> GetCurrentOrThrowAsync();
    }
}
