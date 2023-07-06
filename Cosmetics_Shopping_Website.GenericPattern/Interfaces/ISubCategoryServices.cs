using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface ISubCategoryServices
    {
        
        Task<IEnumerable<Category>> GetCategory();
        Task<SubCategoryVM> CreateSubCategory(string subCategoryName, int categoryId,int logedUser);
        Task<SubCategoryVM> GetSubCategoryByIds(int id);
        Task<IEnumerable<SubCategoryVM>> GetAllSubCategories();
        
        Task<SubCategoryVM> PutSubCategory(int Id,string SubCategoryName, int categoryId,int logedUser);

        Task<bool> DeleteSubCategory(int id, int logedUser);
    }
}
