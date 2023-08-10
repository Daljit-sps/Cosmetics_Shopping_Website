using Microsoft.AspNetCore.Mvc;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Cosmetics_Shopping_Website.Controllers
{
    
    public class CategoriesController : Controller
    {
        public readonly ICategoryServices _categoryServices;
        private readonly IHttpContextAccessor _contextAccessor;

        public CategoriesController(ICategoryServices categoryServices, IHttpContextAccessor contextAccessor)
        {
            _categoryServices = categoryServices;
            _contextAccessor = contextAccessor;
        }


        //CreateCategory
        public async Task<IActionResult> CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(String categoryname)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var categoryCreated = await _categoryServices.CreateCategory(categoryname, logedUser.UserId);
                    if (categoryCreated != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                return View();

            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        //Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var categoriesDetailsList = await _categoryServices.GetAllCategories();

                return View(categoriesDetailsList);
            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        //Details of particular category
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var categoryDetails = await _categoryServices.GetCategoryByIds(id);
                return View(categoryDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        //Edit
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var categoryDetails = await _categoryServices.GetCategoryByIds(id);
               
                return View(categoryDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category objCategory)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var categoryUpdated = await _categoryServices.PutCategory(objCategory, logedUser.UserId);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(objCategory);
                }
            }
            catch (Exception ex)
            {
                return View(ex);
            }

        }

        //Delete
       
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                var categoryDeleted = await _categoryServices.DeleteCategory(Id, logedUser.UserId);
                if (categoryDeleted != true)
                {
                    return View("Error");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

    }
}
