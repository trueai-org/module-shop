using Shop.Module.Core.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        bool ValidateToken(string identityId, string token);

        void RemoveUserToken(int userId);

        Task<IList<Claim>> BuildClaims(User user);
    }
}
