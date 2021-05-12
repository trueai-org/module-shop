using Shop.Module.ShoppingCart.Entities;
using Shop.Module.ShoppingCart.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.ShoppingCart.Services
{
    public interface ICartService
    {
        Task AddToCart(int customerId, int productId, int quantity);

        Task AddToCart(int customerId, int createdById, int productId, int quantity);

        Task UpdateItemQuantity(int customerId, int createdById, int productId, int quantity);

        Task CheckedItem(int customerId, CheckedItemParam model);

        IQueryable<Cart> Query();

        IQueryable<Cart> GetActiveCart(int customerId);

        IQueryable<Cart> GetActiveCart(int customerId, int createdById);

        Task<CartResult> GetActiveCartDetails(int customerId);

        Task<CartResult> GetActiveCartDetails(int customerId, int createdById);

        //Task<CouponValidationResult> ApplyCoupon(int cartId, string couponCode);

        Task MigrateCart(int fromUserId, int toUserId);
    }
}
