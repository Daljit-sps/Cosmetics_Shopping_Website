using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IProductServices
    {
        Task<IEnumerable<Category>> GetCategory();
        Task<IEnumerable<SubCategory>> GetSubCategory();
        Task<ProductVM> CreateProduct(String ProductName, int CategoryId, int SubCategoryId, int logedUser);
        Task<ProductVM> GetProductByIds(int id);
        Task<IEnumerable<ProductVM>> GetAllProducts();
       
        Task<ProductVM> PutProduct(int Id, string ProductName, int CategoryId, int SubCategoryId, int logedUser);

        Task<bool> DeleteProduct(int id, int logedUser);
    }
}
