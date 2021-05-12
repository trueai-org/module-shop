using Shop.Module.Core.Models;
using System;

namespace Shop.Module.Core.Models.Cache
{
    public class UserTokenCache
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public UserTokenType TokenType { get; set; } = UserTokenType.Default;
        public DateTime TokenCreatedOnUtc { get; set; }
        public DateTime TokenUpdatedOnUtc { get; set; }
        public DateTime? TokenExpiresOnUtc { get; set; }
    }
}
