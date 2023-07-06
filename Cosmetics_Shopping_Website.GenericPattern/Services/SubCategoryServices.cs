using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class SubCategoryServices: ISubCategoryServices
    {
        public IGenericRepository _genericRepository;

        public SubCategoryServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

       
        public async Task<IEnumerable<Category>> GetCategory()
        {
            var result= await _genericRepository.GetTable<Category>();
            return result.Where(e=>e.IsDelete == false).ToList();
        }

        public async Task<SubCategoryVM> CreateSubCategory(string subCategoryName, int categoryId,int logedUser)
        {
            try
            {
                SubCategory objSubCategory = new();
                objSubCategory.CategoryId = categoryId;
                objSubCategory.SubCategoryName = subCategoryName;
                objSubCategory.CreatedBy = logedUser;
                objSubCategory.CreatedOn = DateTime.Now;
                objSubCategory.UpdatedOn = DateTime.Now;
                objSubCategory.UpdatedBy = logedUser;

                if (objSubCategory != null)
                {
                    await _genericRepository.Post(objSubCategory);
                    SubCategoryVM result = new()
                    {   Id = objSubCategory.Id,
                        CategoryId=objSubCategory.CategoryId,
                        SubCategoryName=objSubCategory.SubCategoryName
                        
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

        public async Task<SubCategoryVM> GetSubCategoryByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var subcategoryDetails = await _genericRepository.GetByIdFromMultipleTable<SubCategory>(id, e=>e.Category);
                    if (subcategoryDetails != null && subcategoryDetails.IsDelete == false)
                    {
                        SubCategoryVM result = new()
                        {
                            Id=subcategoryDetails.Id,
                            CategoryId = subcategoryDetails.CategoryId,
                            SubCategoryName = subcategoryDetails.SubCategoryName,
                            Category = subcategoryDetails.Category.CategoryName
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

      

        public async Task<IEnumerable<SubCategoryVM>> GetAllSubCategories()
        {
            try
            {
                var subcategoryDetailsList = await _genericRepository.GetFromMutlipleTable<SubCategory>(e=>e.Category);
                return subcategoryDetailsList.Select(e => new SubCategoryVM
                {
                    Id=e.Id,
                    CategoryId= e.CategoryId,
                    SubCategoryName = e.SubCategoryName,
                    Category = e.Category.CategoryName,
                    IsDelete = e.IsDelete
                }).Where(e => e.IsDelete == false).ToList();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<SubCategoryVM> PutSubCategory(int Id,string subCategoryName, int categoryId, int logedUser)
        {
            try
            {
                var objSubCategory = await _genericRepository.GetById<SubCategory>(Id);
                if (objSubCategory != null && objSubCategory.IsDelete == false)
                {
                    objSubCategory.CategoryId = categoryId;
                    objSubCategory.SubCategoryName = subCategoryName;
                    objSubCategory.UpdatedOn = DateTime.Now;
                    objSubCategory.UpdatedBy = logedUser;

                    await _genericRepository.Put(objSubCategory);
                    SubCategoryVM result = new()
                    {
                        Id=objSubCategory.Id,
                        CategoryId = objSubCategory.CategoryId,
                        SubCategoryName = subCategoryName,
                     
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

        public async Task<bool> DeleteSubCategory(int Id, int logedUser)
        {
            if (Id > 0)
            {
                var subcategoryDetails = await _genericRepository.GetById<SubCategory>(Id);
                if (subcategoryDetails != null)
                {
                    subcategoryDetails.IsDelete = true;
                    subcategoryDetails.UpdatedBy = logedUser;
                    subcategoryDetails.UpdatedOn = DateTime.Now;
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
