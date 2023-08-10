using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class ProductVariantServices: IProductVariantServices
    {
        public IGenericRepository _genericRepository;

        public ProductVariantServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<Product>> GetProduct()
        {
            var result = await _genericRepository.GetTable<Product>();
            return result.Where(e => e.IsDelete == false).ToList();
        }

      
        public async Task<ProductVariantVM> CreateProductVariant(ProductVariantVM objProductVariantVM, int logedUser)
        {
            try
            {
                ProductVariant objProductVariant = new();
                objProductVariant.VariantName = objProductVariantVM.VariantName;
                objProductVariant.VariantImage = objProductVariantVM.VariantImage;
                objProductVariant.VariantDescription = objProductVariantVM.VariantDescription;
                objProductVariant.Price = objProductVariantVM.Price;
                objProductVariant.ProductId = objProductVariantVM.ProductId;
                objProductVariant.CreatedBy = logedUser;
                objProductVariant.CreatedOn = DateTime.Now;
                objProductVariant.UpdatedOn = DateTime.Now;
                objProductVariant.UpdatedBy = logedUser;

                if (objProductVariant != null)
                {
                    await _genericRepository.Post(objProductVariant);
                    ProductVariantVM result = JsonConvert.DeserializeObject<ProductVariantVM>(JsonConvert.SerializeObject(objProductVariant))!;
                    return result;
                  
                }
                return null;
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

        


        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariants()
        {
            try
            {
                var productvariantDetailsList = await _genericRepository.GetFromMutlipleTable<ProductVariant>(e => e.Product); 
                return productvariantDetailsList.Select(e=> new ProductVariantVM
                {
                    Id = e.Id,
                    VariantName = e.VariantName,
                    VariantDescription = e.VariantDescription,
                    VariantImage = e.VariantImage,
                    ProductId = e.ProductId,
                    Product = e.Product.ProductName,
                    Price = e.Price,
                    IsDelete = e.IsDelete
                }).Where(e => e.IsDelete == false).ToList();


            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnProducts(string condition)
        {
            try
            {
                var productvariantDetailsList = await _genericRepository.GetFromMultipleTableBasedOnConditions<ProductVariant>(e => e.Product.ProductName == condition, e => e.Product);
                if (productvariantDetailsList == null)
                {
                    return Enumerable.Empty<ProductVariantVM>();
                }
                var result = productvariantDetailsList.Select(e => new ProductVariantVM
                {
                    Id = e.Id,
                    VariantName = e.VariantName,
                    VariantDescription = e.VariantDescription,
                    VariantImage = e.VariantImage,
                    ProductId = e.ProductId,
                    Product = e.Product.ProductName,
                    Price = e.Price,
                    IsDelete = e.IsDelete
                }).Where(e => e.IsDelete == false).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnSubCategoryOrCategory(string condition)
        {
            try
            {
                var getCategory = await _genericRepository.Get<Category>(e => e.CategoryName == condition);
                var getSubCategory = await _genericRepository.Get<SubCategory>(e => e.SubCategoryName == condition);
                if (getCategory != null)
                {
                    var productvariantDetailsList = await _genericRepository.GetFromMultipleTableBasedOnConditions<ProductVariant>(e => e.Product.CategoryId == getCategory.Id, e => e.Product);
                    var result = productvariantDetailsList.Select(e => new ProductVariantVM
                    {
                        Id = e.Id,
                        VariantName = e.VariantName,
                        VariantDescription = e.VariantDescription,
                        VariantImage = e.VariantImage,
                        ProductId = e.ProductId,
                        Product = e.Product.ProductName,
                        CategoryName = getCategory.CategoryName,
                        Price = e.Price,
                        IsDelete = e.IsDelete
                    }).Where(e => e.IsDelete == false).ToList();
                    return result;
                }
                else if (getSubCategory != null)
                {
                    var productvariantDetailsList = await _genericRepository.GetFromMultipleTableBasedOnConditions<ProductVariant>(e => e.Product.SubCategoryId == getSubCategory.Id, e => e.Product);
                    var result = productvariantDetailsList.Select(e => new ProductVariantVM
                    {
                        Id = e.Id,
                        VariantName = e.VariantName,
                        VariantDescription = e.VariantDescription,
                        VariantImage = e.VariantImage,
                        ProductId = e.ProductId,
                        Product = e.Product.ProductName,
                        SubCategoryName = getSubCategory.SubCategoryName,
                        Price = e.Price,
                        IsDelete = e.IsDelete
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
        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnSubCategoryOrCategoryWithPagination(string condition, int pageIndex, int pageSize)
        {
            try
            {
                var getCategory = await _genericRepository.Get<Category>(e=>e.CategoryName == condition);
                var getSubCategory = await _genericRepository.Get<SubCategory>(e=>e.SubCategoryName == condition);
                if(getCategory != null)
                {
                    var productvariantDetailsList = await _genericRepository.GetFromMutlipleTableForPaginationBasedOnCondition<ProductVariant>(pageIndex,pageSize,e => e.Product.CategoryId == getCategory.Id, e => e.Product);
                    var result = productvariantDetailsList.Select(e => new ProductVariantVM
                    {
                        Id = e.Id,
                        VariantName = e.VariantName,
                        VariantDescription = e.VariantDescription,
                        VariantImage = e.VariantImage,
                        ProductId = e.ProductId,
                        Product = e.Product.ProductName,
                        CategoryName = getCategory.CategoryName,
                        Price = e.Price,
                        IsDelete = e.IsDelete
                    }).Where(e => e.IsDelete == false).ToList();
                    return result;
                }
                else if( getSubCategory != null )
                {
                    var productvariantDetailsList = await _genericRepository.GetFromMutlipleTableForPaginationBasedOnCondition<ProductVariant>(pageIndex, pageSize, e => e.Product.SubCategoryId == getSubCategory.Id, e => e.Product);
                    var result = productvariantDetailsList.Select(e => new ProductVariantVM
                    {
                        Id = e.Id,
                        VariantName = e.VariantName,
                        VariantDescription = e.VariantDescription,
                        VariantImage = e.VariantImage,
                        ProductId = e.ProductId,
                        Product = e.Product.ProductName,
                        SubCategoryName = getSubCategory.SubCategoryName,
                        Price = e.Price,
                        IsDelete = e.IsDelete
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

        public async Task<IEnumerable<ProductVariantVM>> GetAllProductVariantsBasedOnProductWithPagination(string condition, int pageIndex, int pageSize)
        {
            try
            {
                var productvariantDetailsList = await _genericRepository.GetFromMutlipleTableForPaginationBasedOnCondition<ProductVariant>(pageIndex, pageSize, e=>e.Product.ProductName == condition,e => e.Product);
                if(productvariantDetailsList == null)
                {
                    return Enumerable.Empty<ProductVariantVM>();
                }
                var result= productvariantDetailsList.Select(e => new ProductVariantVM
                {
                    Id = e.Id,
                    VariantName = e.VariantName,
                    VariantDescription = e.VariantDescription,
                    VariantImage = e.VariantImage,
                    ProductId = e.ProductId,
                    Product = e.Product.ProductName,
                    Price = e.Price,
                    IsDelete = e.IsDelete
                }).Where(e=>e.IsDelete == false).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductVariantVM> PutProductVariant(ProductVariantVM objProductVariantVm, int logedUser)
        {
            try
            { 
                var objProductVariant = await _genericRepository.GetById<ProductVariant>(objProductVariantVm.Id);
                if (objProductVariant != null && objProductVariant.IsDelete == false)
                {

                    objProductVariant.ProductId = objProductVariantVm.ProductId;
                    objProductVariant.VariantName = objProductVariantVm.VariantName;
                    objProductVariant.VariantDescription = objProductVariantVm.VariantDescription;
                    objProductVariant.VariantImage = objProductVariantVm.VariantImage;
                    objProductVariant.Price = objProductVariantVm.Price;
                    objProductVariant.UpdatedOn = DateTime.Now;
                    objProductVariant.UpdatedBy = logedUser;

                    await _genericRepository.Put(objProductVariant); 
                    ProductVariantVM result = JsonConvert.DeserializeObject<ProductVariantVM>(JsonConvert.SerializeObject(objProductVariant))!;

                    return result;

                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public async Task<bool> DeleteProductVariant(int id, int logedUser)
        {
            try
            {
                if (id > 0)
                {
                    var productvariantDetails = await _genericRepository.GetById<ProductVariant>(id);
                    if (productvariantDetails != null)
                    {
                        productvariantDetails.IsDelete = true;
                        productvariantDetails.UpdatedBy = logedUser;
                        productvariantDetails.UpdatedOn = DateTime.Now;
                        var result = _genericRepository.Save();


                        if (result > 0)
                        {
                            return true;
                        }

                    }
                }
                return false;
            }
            catch(Exception)
            {
                throw;
            } 
            
        }

    }
}
