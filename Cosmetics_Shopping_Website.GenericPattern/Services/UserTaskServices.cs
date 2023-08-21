using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class UserTaskServices : IUserTaskServices
    {
        public IGenericRepository _genericRepository;

        public UserTaskServices(IGenericRepository genericRepository) 
        { 
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<ProductVariantVM>> SearchVariants(string searchText)
        {
            try
            {
                if(!string.IsNullOrEmpty(searchText))
                {
                    var getVariantsBySearch = await _genericRepository.SearchFormMultipleTable<ProductVariant>(e=>e.VariantName.Contains(searchText) && e.IsDelete==false, e=>e.Product );
                    if(getVariantsBySearch != null)
                    {
                        var result = getVariantsBySearch.Select(e => new ProductVariantVM()
                        {
                            Id = e.Id,
                            VariantName = e.VariantName,
                            VariantDescription = e.VariantDescription,
                            VariantImage = e.VariantImage,
                            Price = e.Price,
                            ProductId = e.ProductId,
                            Product = e.Product.ProductName
                        }).ToList().Take(8);
                        return result;
                    }
                    return Enumerable.Empty<ProductVariantVM>();
                }
                else
                {
                    return Enumerable.Empty<ProductVariantVM>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductVariantVM>> SearchVariantsForLogedUser(string searchText, int logedUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    var getVariantsBySearch = await _genericRepository.SearchFormMultipleTable<ProductVariant>(e => e.VariantName.Contains(searchText) && e.IsDelete == false, e => e.Product);
                    if (getVariantsBySearch != null)
                    {
                        var getWishlistedItems = await _genericRepository.GetFromMultipleTableBasedOnConditions<Wishlist>(e => e.UserId == logedUser && !e.IsDelete, e => e.User);
                        var wishlistedItemIds = getWishlistedItems.Select(w => w.ProductVariantId).ToHashSet();
                        var result = getVariantsBySearch.Select(e => new ProductVariantVM()
                        {
                            Id = e.Id,
                            VariantName = e.VariantName,
                            VariantDescription = e.VariantDescription,
                            VariantImage = e.VariantImage,
                            Price = e.Price,
                            ProductId = e.ProductId,
                            Product = e.Product.ProductName,
                            IsWishlisted = wishlistedItemIds.Contains(e.Id)
                        }).ToList().Take(8);
                        return result;
                    }
                    return Enumerable.Empty<ProductVariantVM>();
                }
                else
                {
                    return Enumerable.Empty<ProductVariantVM>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Wishlist> Wishlist(int productVariantId, int logedUser)
        {
            try
            {
                var wishlistItem = await _genericRepository.Get<Wishlist>(w => w.ProductVariantId == productVariantId && w.UserId == logedUser);
                if (wishlistItem == null)
                {
                    // Add the product to the user's wishlist
                    Wishlist objwishlistItem = new()
                    {
                        UserId = logedUser,
                        ProductVariantId = productVariantId,
                        CreatedBy = logedUser,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = logedUser,
                        UpdatedOn = DateTime.Now
                    };

                    if (objwishlistItem != null)
                    {
                        var result = await _genericRepository.Post(objwishlistItem);

                        return result;
                    }
                    return null;
                }
                else if(wishlistItem.IsDelete) 
                {
                    wishlistItem.UserId = logedUser;
                    wishlistItem.ProductVariantId = productVariantId;
                    wishlistItem.UpdatedBy = logedUser;
                    wishlistItem.UpdatedOn = DateTime.Now;
                    wishlistItem.IsDelete = false;
                    var result = await _genericRepository.Put(wishlistItem);

                    return result;
                   
                }
                else
                {
                    wishlistItem.UserId = logedUser;
                    wishlistItem.ProductVariantId = productVariantId;
                    wishlistItem.IsDelete = true;
                    wishlistItem.UpdatedBy = logedUser;
                    wishlistItem.UpdatedOn = DateTime.Now;
                    var result = await _genericRepository.Put(wishlistItem);
                    return result;
                }
                
            }

            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<IEnumerable<WishListVM>> GetAllWishlistItems(int logedUser)
        {
            try
            {
                 var wishlistItemsDetailList = await _genericRepository.GetFromMultipleTableBasedOnConditions<Wishlist>(e=>e.UserId == logedUser && !e.IsDelete,e => e.User, e=>e.ProductVariant);
                 return wishlistItemsDetailList.Select(e => new WishListVM
                    {
                        Id = e.Id,
                        VariantName = e.ProductVariant.VariantName,
                        VariantDescription = e.ProductVariant.VariantDescription,
                        VariantImage = e.ProductVariant.VariantImage,
                        ProductVariantId = e.ProductVariantId,
                        UserId = e.UserId,
                        Price = e.ProductVariant.Price,
                        IsDelete = e.IsDelete
                    }).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductVariantVM>> GetSimilarProductVariants(int productId, int variantId)
        {
            try
            {
                if(productId>0)
                {
                    var getSimilarVaraints = await _genericRepository.GetAll<ProductVariant>();
                    var result = getSimilarVaraints.Select(e=>new ProductVariantVM
                    {
                        Id =e.Id,
                        ProductId = e.ProductId,
                        VariantName = e.VariantName,
                        VariantDescription =e.VariantDescription,
                        VariantImage = e.VariantImage,
                        Price = e.Price, 
                        IsDelete = e.IsDelete

                    }).Where(e=>e.ProductId == productId && e.IsDelete == false && e.Id != variantId).ToList().Take(3);
                    if(result.Any())
                    {
                        return result;
                    }
                    else
                    {
                        return Enumerable.Empty<ProductVariantVM>(); 
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception) 
            { 
                throw;
            }
        }


        public async Task<ProductVariantVM> GetProductVariantByIdForLogedUser(int id, int logedUser)
        {
            try
            {
                if (id != 0)
                {
                    var productvariantDetails = await _genericRepository.GetById<ProductVariant>(id);
                    if (productvariantDetails != null && productvariantDetails.IsDelete == false)
                    {
                        var wishlisted = await _genericRepository.Get<Wishlist>(e => e.UserId == logedUser && e.ProductVariantId == id && !e.IsDelete);
                        ProductVariantVM result = new()
                        {
                            Id = productvariantDetails.Id,
                            ProductId = productvariantDetails.ProductId,
                            VariantName = productvariantDetails.VariantName,
                            VariantDescription = productvariantDetails.VariantDescription,
                            VariantImage = productvariantDetails.VariantImage,
                            IsWishlisted = wishlisted != null,
                            Price = productvariantDetails.Price
                        };

                        return result;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnProductWithPaginationForLogedUser(int logedUser, string condition, int pageIndex, int pageSize)
        {
            try
            {
                var productvariantDetails = await _genericRepository.GetFromMutlipleTableForPaginationBasedOnCondition<ProductVariant>(pageIndex, pageSize, e => e.Product.ProductName == condition && e.IsDelete == false, e => e.Product);

                if (productvariantDetails.Count() > 0)
                {
                    var result = new List<ProductVariantVM>();
                    var getWishlistedItems = await _genericRepository.GetFromMultipleTableBasedOnConditions<Wishlist>(e=>e.UserId == logedUser && !e.IsDelete, e=>e.User);
                    var wishlistedItemIds = getWishlistedItems.Select(w => w.ProductVariantId).ToHashSet();
                    foreach (var productvariant in productvariantDetails)
                    {
                        result.Add(new ProductVariantVM
                        {
                            Id = productvariant.Id,
                            ProductId = productvariant.ProductId,
                            VariantName = productvariant.VariantName,
                            VariantDescription = productvariant.VariantDescription,
                            VariantImage = productvariant.VariantImage,
                            IsWishlisted = wishlistedItemIds.Contains(productvariant.Id),
                            Price = productvariant.Price
                        });
                    }

                    

                    return result;
                }
                else
                {

                    return Enumerable.Empty<ProductVariantVM>();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnSubCategoryOrCategoryWithPaginationForLogedUser(int logedUser, string condition, int pageIndex, int pageSize)
        {
            try
            {
                var getCategory = await _genericRepository.Get<Category>(e => e.CategoryName == condition);
                var getSubCategory = await _genericRepository.Get<SubCategory>(e => e.SubCategoryName == condition);

                if (getCategory != null || getSubCategory != null)
                {
                    var isCategory = getCategory != null;
                    var categoryId = isCategory ? getCategory.Id : (int?)null;
                    var subCategoryId = !isCategory ? getSubCategory.Id : (int?)null;

                    var productvariantDetailsList = await _genericRepository.GetFromMutlipleTableForPaginationBasedOnCondition<ProductVariant>(pageIndex, pageSize, e =>
                        (categoryId.HasValue && e.Product.CategoryId == categoryId) ||
                        (subCategoryId.HasValue && e.Product.SubCategoryId == subCategoryId),
                        e => e.Product);

                    var getWishlistedItems = await _genericRepository.GetFromMultipleTableBasedOnConditions<Wishlist>(e => e.UserId == logedUser && !e.IsDelete, e => e.User);
                    var wishlistedItemIds = getWishlistedItems.Select(w => w.ProductVariantId).ToHashSet();

                    var result = productvariantDetailsList.Select(e => new ProductVariantVM
                    {
                        Id = e.Id,
                        VariantName = e.VariantName,
                        VariantDescription = e.VariantDescription,
                        VariantImage = e.VariantImage,
                        ProductId = e.ProductId,
                        Product = e.Product.ProductName,
                        CategoryName = isCategory ? getCategory.CategoryName : null,
                        SubCategoryName = !isCategory ? getSubCategory.SubCategoryName : null,
                        Price = e.Price,
                        IsDelete = e.IsDelete,
                        IsWishlisted = wishlistedItemIds.Contains(e.Id)
                    }).Where(e => e.IsDelete == false).ToList();

                    return result;
                }
                else
                {
                    return Enumerable.Empty<ProductVariantVM>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

      
        public async Task<ProductVariantVM> GetProductVariantById(int id)
        {
            try
            {
                if (id != 0)
                {
                    var productvariantDetails = await _genericRepository.GetById<ProductVariant>(id);
                    if (productvariantDetails != null && productvariantDetails.IsDelete == false)
                    {
                        ProductVariantVM result = new()
                        {
                            Id = productvariantDetails.Id,
                            ProductId = productvariantDetails.ProductId,
                            VariantName = productvariantDetails.VariantName,
                            VariantDescription = productvariantDetails.VariantDescription,
                            VariantImage = productvariantDetails.VariantImage,
                            Price = productvariantDetails.Price
                        };

                        return result;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<CartItem> AddToCart(int productVariantId, int quantity,int logedUser)
        {
            try
            {
                var cartItemDetails = await _genericRepository.Get<CartItem>(e=>e.ProductVariantId == productVariantId && e.UserId == logedUser);
                if (cartItemDetails == null)
                {
                    // Add the product to the user's cart
                    CartItem objcartItem = new()
                    {
                        UserId = logedUser,
                        ProductVariantId = productVariantId,
                        CreatedBy = logedUser,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = logedUser,
                        UpdatedOn = DateTime.Now
                    };
                    var result = await _genericRepository.Post(objcartItem);

                    return result;
                   
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        public async Task<CartItem> UpdateCartItemQuantity(int cartItemId, int quantity, int logedUser)
        {
            try
            {
                var cartItemDetails = await _genericRepository.GetById<CartItem>(cartItemId);
                if (cartItemDetails != null)
                {
                    cartItemDetails.Quantity = quantity;
                    cartItemDetails.UpdatedOn = DateTime.Now;
                    cartItemDetails.UpdatedBy = logedUser;
                    var result= await _genericRepository.Put(cartItemDetails);
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }


        public async Task<string> RemoveFromCart(int cartItemId, int logedUser)
        {
            try
            {
                var cartItem = await _genericRepository.Get<CartItem>(e=>e.Id == cartItemId && e.UserId == logedUser);
                string Response = string.Empty;
                if (cartItem != null)
                {
                   var result = _genericRepository.Delete<CartItem>(cartItem);
                   Response = "pass";
                }
                return Response;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<string> RemoveAllItemsFromCart(int logedUser)
        {
            try
            {
                var cartItemsDetailList = await _genericRepository.GetFromMultipleTableBasedOnConditions<CartItem>(e => e.UserId == logedUser, e => e.User, e => e.ProductVariant);
                string Response = string.Empty;
                if (cartItemsDetailList != null)
                {
                    var result = _genericRepository.DeleteAllRowsOnCondition<CartItem>(e => e.UserId == logedUser);
                    Response = "pass";
                }
                return Response;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<CartItemsVM>> GetAllCartItems(int logedUser)
        {
            try
            {
                var cartItemsDetailList = await _genericRepository.GetFromMultipleTableBasedOnConditions<CartItem>(e => e.UserId == logedUser, e => e.User, e => e.ProductVariant);
                return cartItemsDetailList.Select(e => new CartItemsVM
                {
                    Id = e.Id,
                    VariantName = e.ProductVariant.VariantName,
                    VariantImage = e.ProductVariant.VariantImage,
                    ProductVariantId = e.ProductVariantId,
                    Quantity = e.Quantity,
                    UserId = e.UserId,
                    Price = e.ProductVariant.Price,
                    CartItemPrice = e.ProductVariant.Price * e.Quantity,
                }).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<CartItemsVM>> GetVariantsListPresentInCart(IEnumerable<CartItemsVM> cartItemsList)
        {
            try
            {
                var productVariantsWithQuantities = cartItemsList.GroupBy(item => new { item.ProductVariantId, item.Quantity})
                    .Select(group => new CartItem
                    {
                        ProductVariantId = group.Key.ProductVariantId,
                        Quantity = group.Sum(item => item.Quantity)
                    }).ToList();
                return productVariantsWithQuantities.Select(e=> new CartItemsVM
                {
                    ProductVariantId= e.ProductVariantId,
                    Quantity = e.Quantity,
                    //Price = e.ProductVariant.Price
                }).ToList();
              
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Country> GetCountry()
        {
            var result = await _genericRepository.GetTable<Country>();
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<State>> GetStates()
        {
            var result = await _genericRepository.GetTable<State>();
            return result.Where(e => e.IsDelete == false).ToList();
        }

        public async Task<UserAddressVM> GetLogedUserDetails(int logedUser)
        {
            try
            {
                if (logedUser != 0)
                {
                    var userDetails = await _genericRepository.GetById<User>(logedUser);
                    if (userDetails != null && userDetails.IsDelete == false)
                    {
                        UserAddressVM result = new()
                        {
                            FullName = userDetails.FirstName+" " +userDetails.LastName,
                            MobileNumber = userDetails.MobileNumber,
                            Email = userDetails.Email
                        };
                        return result;
                    }
                }
                return null;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<UserAddressVM> AddUserShippingAddress(UserAddressVM objUserShippingAddress, int logedUser)
        {
            try
            {
                var userShippingAddressDetails = await _genericRepository.Get<UserAddress>(e => e.Address == objUserShippingAddress.Address && e.City ==objUserShippingAddress.City && e.StateId == objUserShippingAddress.StateId && e.UserId == logedUser && e.IsDelete == false);
                if (userShippingAddressDetails == null && objUserShippingAddress.Country == "India")
                {
                    UserAddress objAddress = new()
                    {
                        UserId = logedUser,
                        City = objUserShippingAddress.City,
                        PostalCode = objUserShippingAddress.PostalCode,
                        Address = objUserShippingAddress.Address,
                        StateId = objUserShippingAddress.StateId,
                        IsDefault = objUserShippingAddress.IsDefault,
                        CreatedBy = logedUser,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = logedUser,
                        UpdatedOn = DateTime.Now
                    };
                     await _genericRepository.Post(objAddress);
                    UserAddressVM result = new()
                    {
                        City = objUserShippingAddress.City,
                        Address = objUserShippingAddress.Address,
                        PostalCode = objUserShippingAddress.PostalCode,
                        StateId = objUserShippingAddress.StateId,
                    };
                    return result;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<UserAddressVM> EditUserShippingAddress(UserAddressVM objUserShippingAddress, int logedUser)
        {
            try
            {
                var userShippingAddressDetails = await _genericRepository.Get<UserAddress>(e => e.Id == objUserShippingAddress.Id && e.UserId == logedUser && e.IsDelete == false);
                var getState = await _genericRepository.Get<State>(e => e.Id == objUserShippingAddress.StateId);
                if (userShippingAddressDetails != null && objUserShippingAddress.Country == "India")
                { 
                    userShippingAddressDetails.City = objUserShippingAddress.City;
                    userShippingAddressDetails.PostalCode = objUserShippingAddress.PostalCode;
                    userShippingAddressDetails.Address = objUserShippingAddress.Address;
                    userShippingAddressDetails.StateId = objUserShippingAddress.StateId;
                    userShippingAddressDetails.IsDefault = objUserShippingAddress.IsDefault;                   
                    userShippingAddressDetails.UpdatedBy = logedUser;
                    userShippingAddressDetails.UpdatedOn = DateTime.Now;
                   
                    await _genericRepository.Put(userShippingAddressDetails);
                    UserAddressVM result = new()
                    {
                        Id = userShippingAddressDetails.Id,
                        UserId = userShippingAddressDetails.UserId,
                        City= userShippingAddressDetails.City,
                        PostalCode= userShippingAddressDetails.PostalCode,  
                        Address = userShippingAddressDetails.Address,
                        StateId = userShippingAddressDetails.StateId,
                        State = getState.StateName,
                        IsDefault = userShippingAddressDetails.IsDefault,
                    };
                    return result;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<UserAddressVM> GetUserShippingAddressById(int addressId, int logedUser)
        {
            try
            {
                if (addressId>0 && logedUser > 0)
                {
                    var userAddressDetails = await _genericRepository.Get<UserAddress>(e=>e.Id == addressId && e.IsDelete == false);
                    var userDetails = await _genericRepository.GetById<User>(logedUser);
                    if (userAddressDetails != null)
                    {
                        UserAddressVM result = new()
                        {
                            Id = userAddressDetails.Id,
                            UserId = userAddressDetails.UserId,
                            FullName = userDetails.FirstName + " " + userDetails.LastName,
                            MobileNumber = userDetails.MobileNumber,
                            Email = userDetails.Email,
                            Address = userAddressDetails.Address,
                            City = userAddressDetails.City,
                            PostalCode = userAddressDetails.PostalCode,
                            StateId = userAddressDetails.StateId,
                            State = userAddressDetails.State.StateName,
                            IsDefault = userAddressDetails.IsDefault
                        };
                        return result;
                    }
                }
                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<UserAddressVM>> GetUserAddresses(int logedUser)
        {
            try
            {
                var userAddressesDetailList = await _genericRepository.GetFromMultipleTableBasedOnConditions<UserAddress>(e => e.UserId == logedUser, e => e.State);
                return userAddressesDetailList.Select(e => new UserAddressVM
                {
                    Id = e.Id,
                    StateId = e.StateId,
                    State = e.State.StateName,
                    City = e.City,
                    Address = e.Address,
                    PostalCode = e.PostalCode,
                    IsDefault = e.IsDefault,
                    IsDelete = e.IsDelete
                }).Where(e=>e.IsDelete == false).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> DeleteUserAddress(int addressId, int logedUser)
        {
            try
            {
                var userAddress = await _genericRepository.Get<UserAddress>(e => e.Id == addressId && e.UserId == logedUser);
                if (userAddress != null)
                {
                    userAddress.IsDelete = true;
                    userAddress.UpdatedBy = logedUser;
                    userAddress.UpdatedOn = DateTime.Now;
                    var result = _genericRepository.Save();


                    if (result > 0)
                    {
                        return true;
                    }

                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
