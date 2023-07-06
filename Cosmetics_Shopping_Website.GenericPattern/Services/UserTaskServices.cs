using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.EntityFrameworkCore;
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
                else if(wishlistItem.IsDelete == true)
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
                 var wishlistItemsDetailList = await _genericRepository.GetFromMultipleTableBasedOnConditions<Wishlist>(e=>e.UserId == logedUser,e => e.User, e=>e.ProductVariant);
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
                    }).Where(e => e.IsDelete == false).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductVariantVM> GetProductVariantByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var productvariantDetails = await _genericRepository.GetById<ProductVariant>(id);
                    if (productvariantDetails != null && productvariantDetails.IsDelete == false)
                    {
                        var abc = await _genericRepository.Get<Wishlist>(e => e.UserId == 5 && e.ProductVariantId == id && !e.IsDelete);
                        ProductVariantVM result = new()
                        {
                            Id = productvariantDetails.Id,
                            ProductId = productvariantDetails.ProductId,
                            VariantName = productvariantDetails.VariantName,
                            VariantDescription = productvariantDetails.VariantDescription,
                            VariantImage = productvariantDetails.VariantImage,
                            IsWishlisted = abc != null,
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
        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariants()
        {
            try
            {
                var productvariantDetailsList = await _genericRepository.GetFromMutlipleTable<ProductVariant>(e => e.Product);
                return productvariantDetailsList.Select(e => new ProductVariantVM
                {
                    Id = e.Id,
                    VariantName = e.VariantName,
                    VariantDescription = e.VariantDescription,
                    VariantImage = e.VariantImage,
                    ProductId = e.ProductId,
                    Product = e.Product.ProductName,
                    Price = e.Price,
                    IsDelete = e.IsDelete,
                    
                }).Where(e => e.IsDelete == false).ToList();


            }
            catch (Exception)
            {
                throw;
            }

        }


    }
}
