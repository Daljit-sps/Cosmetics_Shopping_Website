using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IProductVariantServices
    {
        Task<IEnumerable<Product>> GetProduct();
       
        Task<ProductVariantVM> CreateProductVariant(ProductVariantVM objProductVariantVM, int logedUser);
        Task<ProductVariantVM> GetProductVariantByIds(int id);
        Task<IEnumerable<ProductVariantVM>> GetAllProductVariants();
      
        Task<ProductVariantVM> PutProductVariant(ProductVariantVM objProductVariantVM, int logedUser);

        Task<bool> DeleteProductVariant(int id, int logedUser);
    }
}
