using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System.Drawing.Printing;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IUserTaskServices
    {
        Task<IEnumerable<ProductVariantVM>> SearchVariants(string searchText);

        Task<IEnumerable<ProductVariantVM>> SearchVariantsForLogedUser(string searchText, int logedUser);

        Task<ProductVariantVM> GetProductVariantById(int id);

        Task<ProductVariantVM> GetProductVariantByIdForLogedUser(int id, int logedUser);

        Task<Wishlist> Wishlist(int productVariantId, int logedUser);
       

        Task<IEnumerable<WishListVM>> GetAllWishlistItems(int logedUser);

        Task<IEnumerable<ProductVariantVM>> GetSimilarProductVariants(int productId, int variantId);


        Task<CartItem> AddToCart(int productVariantId, int quantity,int logedUser);

        Task<CartItem> UpdateCartItemQuantity(int cartItemId, int quantity, int logedUser);

        Task<string> RemoveFromCart(int cartItemId, int logedUser);

        Task<string> RemoveAllItemsFromCart(int logedUser);

        Task<IEnumerable<CartItemsVM>> GetAllCartItems(int logedUser);

        Task<IEnumerable<CartItemsVM>> GetVariantsListPresentInCart(IEnumerable<CartItemsVM> cartItemsList);

        Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnProductWithPaginationForLogedUser(int logedUser, string condition,int pageIndex, int pageSize);

        Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnSubCategoryOrCategoryWithPaginationForLogedUser(int logedUser, string condition, int pageIndex, int pageSize);

        Task<Country> GetCountry();

        Task<IEnumerable<State>> GetStates();

        Task<UserAddressVM> GetLogedUserDetails(int logedUser);

        Task<UserAddress> AddUserShippingAddress(UserAddressVM objUserShippingAddress, int logedUser);

        Task<UserAddressVM> EditUserShippingAddress(UserAddressVM objUserShippingAddress, int logedUser);
       
        Task<bool> DeleteUserAddress(int addressId, int logedUser);

        Task<UserAddressVM> GetUserShippingAddressById(int addressId, int logedUser);

        Task<IEnumerable<UserAddressVM>> GetUserAddresses(int logedUser);


    }
}
