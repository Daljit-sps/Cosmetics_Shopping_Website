using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class ProductServices : IProductServices
    {
        public IGenericRepository _genericRepository;

        public ProductServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<Category>> GetCategory()
        {
            var result = await _genericRepository.GetTable<Category>();
            return result.Where(e => e.IsDelete == false).ToList();
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategory()
        {
            var result = await _genericRepository.GetTable<SubCategory>();
            return result.Where(e => e.IsDelete == false).ToList();
        }
        public async Task<ProductVM> CreateProduct(string ProductName, int CategoryId, int SubCategoryId, int logedUser)
        {
            try
            {
                Product objProduct = new();
                objProduct.CategoryId= CategoryId;
                objProduct.SubCategoryId= SubCategoryId;
                objProduct.ProductName = ProductName;
                objProduct.CreatedBy = logedUser;
                objProduct.CreatedOn = DateTime.Now;
                objProduct.UpdatedOn = DateTime.Now;
                objProduct.UpdatedBy = logedUser;

                if (objProduct != null)
                {
                    await _genericRepository.Post(objProduct);

                    ProductVM result = new()
                    {
                        Id = objProduct.Id,
                        CategoryId = objProduct.CategoryId,
                        SubCategoryId=objProduct.SubCategoryId,
                        ProductName = ProductName,
                       
                    };
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ProductVM> GetProductByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var productDetails = await _genericRepository.GetByIdFromMultipleTable<Product>(id, e=>e.Category, e=>e.SubCategory);
                    if (productDetails != null && productDetails.IsDelete == false)
                    {
                        ProductVM result = new()
                        {
                            Id = productDetails.Id,
                            CategoryId = productDetails.CategoryId,
                            SubCategoryId = productDetails.SubCategoryId,
                            ProductName = productDetails.ProductName,
                            Category = productDetails.Category.CategoryName,
                            SubCategory = productDetails.SubCategory.SubCategoryName
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

      


        public async Task<IEnumerable<ProductVM>> GetAllProducts()
        {
            try
            {
                var productDetailsList = await _genericRepository.GetFromMutlipleTable<Product>(e=>e.Category,e=>e.SubCategory);
                return productDetailsList.Select(e => new ProductVM
                {
                    Id = e.Id,
                    CategoryId = e.CategoryId,
                    SubCategoryId = e.SubCategoryId,
                    Category = e.Category.CategoryName,
                    SubCategory = e.SubCategory.SubCategoryName,
                    ProductName = e.ProductName,
                    IsDelete = e.IsDelete
                }).Where(e => e.IsDelete == false).ToList();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ProductVM> PutProduct(int Id, string ProductName, int CategoryId, int SubCategoryId, int logedUser)
        {
            try
            {
                var objProduct = await _genericRepository.GetById<Product>(Id);
                if (objProduct != null && objProduct.IsDelete == false)
                {
                    objProduct.Id = Id;
                    objProduct.CategoryId = CategoryId;
                    objProduct.SubCategoryId = SubCategoryId;
                    objProduct.ProductName = ProductName;
                    objProduct.UpdatedOn = DateTime.Now;
                    objProduct.UpdatedBy = logedUser;

                    await _genericRepository.Put(objProduct);
                    ProductVM result = new()
                    {
                        Id = objProduct.Id,
                        CategoryId = objProduct.CategoryId,
                        SubCategoryId = objProduct.SubCategoryId,
                        ProductName= objProduct.ProductName,
                        
                    };
                    return result;

                }

            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<bool> DeleteProduct(int id, int logedUser)
        {
            if (id > 0)
            {
                var productDetails = await _genericRepository.GetById<Product>(id);
                if (productDetails != null)
                {
                    productDetails.IsDelete = true;
                    productDetails.UpdatedBy = logedUser;
                    productDetails.UpdatedOn = DateTime.Now;
                    var result = _genericRepository.Save();


                    if (result > 0)
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
