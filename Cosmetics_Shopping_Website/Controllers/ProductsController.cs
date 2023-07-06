using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class ProductsController : Controller
    {
        public readonly IProductServices _productServices;
        private readonly IHttpContextAccessor _contextAccessor;
        public ProductsController(IProductServices productServices, IHttpContextAccessor contextAccessor)
        {
            _productServices = productServices;
            _contextAccessor = contextAccessor;
        }
        //Create Product
        public async Task<IActionResult> Create()
        {
            var selectListItemsForCategory = await _productServices.GetCategory();
            var selectListItemsForSubCategory = await _productServices.GetSubCategory();
            
            //dropdown to select Category
            var selectListForCategory = new SelectList(selectListItemsForCategory, "Id", "CategoryName");
            ViewBag.DropDownDataForCategory = selectListForCategory;

            //dropdown to select Sub-Category
            var selectListForSubCategory = new SelectList(selectListItemsForSubCategory, "Id", "SubCategoryName");
            ViewBag.DropDownDataForSubCategory = selectListForSubCategory;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<ProductVM>> Create(string ProductName, int CategoryId, int SubCategoryId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var productCreated = await _productServices.CreateProduct(ProductName, CategoryId, SubCategoryId, logedUser.UserId);
                    if (productCreated != null)
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
                var productDetailsList = await _productServices.GetAllProducts();
                return View(productDetailsList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        //Details of particular product
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var productDetails = await _productServices.GetProductByIds(id);
                return View(productDetails);
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
                var selectListItemsForCategory = await _productServices.GetCategory();
                var selectListItemsForSubCategory = await _productServices.GetSubCategory();

                //dropdown to select Category
                var selectListForCategory = new SelectList(selectListItemsForCategory, "Id", "CategoryName");
                ViewBag.DropDownDataForCategory = selectListForCategory;
                var selectListForSubCategory = new SelectList(selectListItemsForSubCategory, "Id", "SubCategoryName");

                //dropdown to select Sub-Category
                ViewBag.DropDownDataForSubCategory = selectListForSubCategory;
                var productDetails = await _productServices.GetProductByIds(id);
                return View(productDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int Id, string ProductName, int CategoryId, int SubCategoryId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    var productUpdated = await _productServices.PutProduct(Id, ProductName, CategoryId, SubCategoryId, logedUser.UserId);
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
                
                var productDeleted = await _productServices.DeleteProduct(Id, logedUser.UserId);
                if (productDeleted != true)
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
