using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IUserTaskServices
    {
        Task<Wishlist> Wishlist(int productVariantId, int logedUser);

        Task<IEnumerable<WishListVM>> GetAllWishlistItems(int logedUser);

        Task<ProductVariantVM> GetProductVariantByIds(int id);

    }
}
