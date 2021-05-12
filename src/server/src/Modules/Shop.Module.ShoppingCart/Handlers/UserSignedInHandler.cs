using MediatR;
using Shop.Module.Core.Events;
using Shop.Module.Core.Extensions;
using Shop.Module.ShoppingCart.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.ShoppingCart.Handlers
{
    public class UserSignedInHandler : INotificationHandler<UserSignedIn>
    {
        private readonly IWorkContext _workContext;
        private readonly ICartService _cartService;

        public UserSignedInHandler(IWorkContext workContext, ICartService cartService)
        {
            _workContext = workContext;
            _cartService = cartService;
        }

        public async Task Handle(UserSignedIn user, CancellationToken cancellationToken)
        {
            var guestUser = await _workContext.GetCurrentUserAsync();
            await _cartService.MigrateCart(guestUser.Id, user.UserId);
        }
    }
}
