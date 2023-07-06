using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.Services;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class ProductVariantsController : Controller
    {
        public readonly IProductVariantServices _productvariantServices;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductVariantsController(IProductVariantServices productvariantServices, IHttpContextAccessor contextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _productvariantServices = productvariantServices;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
        //Create ProductVariant
        public async Task<IActionResult> Create()
        {
            var selectListItemsForProduct = await _productvariantServices.GetProduct();

            //dropdown to select Product
            var selectListForProduct = new SelectList(selectListItemsForProduct, "Id", "ProductName");
            ViewBag.DropDownDataForProduct = selectListForProduct;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult<ProductVariantVM>> Create(ProductVariantVM objProductVariantVM)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (ModelState.IsValid)
                {
                    string uniqueFileName = null;
                    if (objProductVariantVM.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + objProductVariantVM.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        objProductVariantVM.ImageFile.CopyTo(new FileStream(filePath, FileMode.Create));

                        objProductVariantVM.VariantImage = uniqueFileName;
                        var productvariantCreated = await _productvariantServices.CreateProductVariant(objProductVariantVM, logedUser.UserId);
                        if (productvariantCreated != null)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            return View("Error");
                        }
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
                var productvariantDetailsList = await _productvariantServices.GetAllProductVariants();
                return View(productvariantDetailsList);
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
                var productvariantDetails = await _productvariantServices.GetProductVariantByIds(id);
                return View(productvariantDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        //Edit
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                //dropdown to select Category
                var selectListItemsForProduct = await _productvariantServices.GetProduct();
                var selectListForProduct = new SelectList(selectListItemsForProduct, "Id", "ProductName");
                ViewBag.DropDownDataForProduct = selectListForProduct;
               
                var productvariantDetails = await _productvariantServices.GetProductVariantByIds(Id);
                return View(productvariantDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductVariantVM objProductVariantVM)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                if (ModelState.IsValid)
                {
                    string uniqueFileName = null;
                    if (objProductVariantVM.ImageFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + objProductVariantVM.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        objProductVariantVM.ImageFile.CopyTo(new FileStream(filePath, FileMode.Create));

                        objProductVariantVM.VariantImage = uniqueFileName;
                        var productvariantUpdated = await _productvariantServices.PutProductVariant(objProductVariantVM, logedUser.UserId);
                        if (productvariantUpdated != null)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            return View("Error");
                        }
                    }
                   
                }
                return View();
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

                var productvariantDeleted = await _productvariantServices.DeleteProductVariant(Id, logedUser.UserId);
                if (productvariantDeleted != true)
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
