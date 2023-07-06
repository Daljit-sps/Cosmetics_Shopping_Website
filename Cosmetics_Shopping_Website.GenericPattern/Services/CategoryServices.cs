using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class CategoryServices: ICategoryServices
    {
        public IGenericRepository _genericRepository;

        public CategoryServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<Category> CreateCategory(string categoryname, int logedUser)
        {
            try
            {
                Category objCategory = new();
                objCategory.CategoryName = categoryname;
                objCategory.CreatedBy = logedUser;
                objCategory.CreatedOn = DateTime.Now;
                objCategory.UpdatedOn = DateTime.Now;
                objCategory.UpdatedBy = logedUser;
               
                if (objCategory != null)
                {
                    var result = await _genericRepository.Post(objCategory);

                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Category> GetCategoryByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var categoryDetails = await _genericRepository.GetById<Category>(id);
                    if (categoryDetails != null && categoryDetails.IsDelete == false)
                    {
                        return categoryDetails;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

       

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try
            {
                var categoryDetailsList = await _genericRepository.GetAll<Category>();
                return categoryDetailsList.Where(e=>e.IsDelete == false).ToList();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Category> PutCategory(Category objPutCategory, int logedUser)
        {
            try
            {
                var objCategory = await _genericRepository.GetById<Category>(objPutCategory.Id);
                if (objCategory != null && objCategory.IsDelete == false)
                {
                    objCategory.Id = objPutCategory.Id;
                    objCategory.CategoryName = objPutCategory.CategoryName;
                    objCategory.UpdatedOn = DateTime.Now;
                    objCategory.UpdatedBy = logedUser;

                    var result= await _genericRepository.Put(objCategory);
                    
                    return result;

                }

            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<bool> DeleteCategory(int id, int logedUser)
        {
            if (id > 0)
            {
                var categoryDetails = await _genericRepository.GetById<Category>(id);
                if (categoryDetails != null)
                {
                    categoryDetails.IsDelete = true;
                    categoryDetails.UpdatedBy = logedUser;
                    categoryDetails.UpdatedOn = DateTime.Now;
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
