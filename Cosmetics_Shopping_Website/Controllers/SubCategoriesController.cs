using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class SubCategoriesController : Controller
    {
        public readonly ISubCategoryServices _subcategoryServices;
        private readonly IHttpContextAccessor _contextAccessor;

        public SubCategoriesController(ISubCategoryServices subcategoryServices, IHttpContextAccessor contextAccessor)
        {
            _subcategoryServices = subcategoryServices;
            _contextAccessor = contextAccessor;
        }

        //Create SubCategory
        public async Task<IActionResult> Create()
        {
            //dropdown to select Category
            var selectListItems = await _subcategoryServices.GetCategory();
            var selectList = new SelectList(selectListItems, "Id", "CategoryName");
            ViewBag.DropDownData = selectList;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<SubCategoryVM>> Create(String subCategoryName, int categoryId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var subcategoryCreated = await _subcategoryServices.CreateSubCategory(subCategoryName, categoryId, logedUser.UserId);
                    if (subcategoryCreated != null)
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
                var subcategoriesDetailsList = await _subcategoryServices.GetAllSubCategories();
                return View(subcategoriesDetailsList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        //Details of particular sub-category
        public async Task<IActionResult> Details(int id)
        {
            try
            {
              
                var subcategoryDetails = await _subcategoryServices.GetSubCategoryByIds(id);
                return View(subcategoryDetails);
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
                //dropdown to select Category
                var selectListItems = await _subcategoryServices.GetCategory();
                var selectList = new SelectList(selectListItems, "Id", "CategoryName");
                ViewBag.DropDownData = selectList;
                
                var subcategoryDetails = await _subcategoryServices.GetSubCategoryByIds(id);
                return View(subcategoryDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int Id, String SubCategoryName, int CategoryId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var subcategoryUpdated = await _subcategoryServices.PutSubCategory(Id, SubCategoryName, CategoryId, logedUser.UserId);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
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
                
                var subcategoryDeleted = await _subcategoryServices.DeleteSubCategory(Id, logedUser.UserId);
                if (subcategoryDeleted != true)
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
