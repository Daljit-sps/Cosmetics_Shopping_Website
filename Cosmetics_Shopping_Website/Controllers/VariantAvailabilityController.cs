using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class VariantAvailabilityController : Controller
    {
        public readonly IVariantAvailabilityServices _variantAvailabilityServices;
        private readonly IHttpContextAccessor _contextAccessor;

        public VariantAvailabilityController(IVariantAvailabilityServices variantAvailabilityServices, IHttpContextAccessor contextAccessor)
        {
            _variantAvailabilityServices = variantAvailabilityServices;
            _contextAccessor = contextAccessor;
        }

        //Create 
        public async Task<IActionResult> Create()
        {
            //dropdown to select Product-Varaint
            var selectListItemsForVariant = await _variantAvailabilityServices.GetProductVariants();
            var selectListForVariant = new SelectList(selectListItemsForVariant, "Id", "VariantName");
            ViewBag.DropDownDataForVariant = selectListForVariant;

            //dropdown to select State
            var selectListItemsForState = await _variantAvailabilityServices.GetStates();
            var selectListForState = new SelectList(selectListItemsForState, "Id", "StateName");
            ViewBag.DropDownDataForState = selectListForState;

            return View();
        }
        [HttpPost]
        public async Task<ActionResult<VariantAvailabilityVM>> Create(VariantAvailabilityVM objVariantAvailabilityVM)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                if (ModelState.IsValid)
                {
                    var variantavailabilityCreated = await _variantAvailabilityServices.CreateVariantAvailability(objVariantAvailabilityVM, logedUser.UserId);
                    if (variantavailabilityCreated != null)
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
                var variantavailabilityDetailsList = await _variantAvailabilityServices.GetAllVariantsAvailability();
                return View(variantavailabilityDetailsList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        //Details 
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var variantavailabilityDetails = await _variantAvailabilityServices.GetVariantAvailabilityByIds(id);
                return View(variantavailabilityDetails);
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
                //dropdown to select Product-Varaint
                var selectListItemsForVariant = await _variantAvailabilityServices.GetProductVariants();
                var selectListForVariant = new SelectList(selectListItemsForVariant, "Id", "VariantName");
                ViewBag.DropDownDataForVariant = selectListForVariant;

                //dropdown to select State
                var selectListItemsForState = await _variantAvailabilityServices.GetStates();
                var selectListForState = new SelectList(selectListItemsForState, "Id", "StateName");
                ViewBag.DropDownDataForState = selectListForState;


                var variantavailabilityDetails = await _variantAvailabilityServices.GetVariantAvailabilityByIds(id);
                return View(variantavailabilityDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(VariantAvailabilityVM objVariantAvailabilityVM)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                if (ModelState.IsValid)
                {
                    var variantavailabilityUpdated = await _variantAvailabilityServices.PutVariantAvailability(objVariantAvailabilityVM, logedUser.UserId);
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

                var variantavailabilityDeleted = await _variantAvailabilityServices.DeleteVariantAvailability(Id, logedUser.UserId);
                if (variantavailabilityDeleted != true)
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
