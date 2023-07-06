using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface ICategoryServices
    {
        Task<Category> CreateCategory(String categoryname, int logedUser);
        Task<Category> GetCategoryByIds(int id);
        Task<IEnumerable<Category>> GetAllCategories();
       
        Task<Category> PutCategory(Category objPutCategory, int logedUser);

        Task<bool> DeleteCategory(int id, int logedUser);
       

    }
}
