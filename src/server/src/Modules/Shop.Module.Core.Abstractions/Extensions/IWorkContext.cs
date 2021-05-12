using Shop.Module.Core.Entities;
using System.Threading.Tasks;

namespace Shop.Module.Core.Extensions
{
    public interface IWorkContext
    {
        Task<User> GetCurrentUserAsync();

        Task<User> GetCurrentUserOrNullAsync();

        Task<User> GetCurrentOrThrowAsync();

        /// <summary>
        /// 验证令牌并自动续签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool ValidateToken(int userId, string token, out int statusCode, string path = "");
    }
}
