using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.Services;
using NuGet.Protocol.Plugins;
using System.Linq;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class HomeController : Controller
    {
        public readonly IProductVariantServices _productvariantServices;
        public readonly IUserTaskServices _userTaskServices;
        private readonly IHttpContextAccessor _contextAccessor;
       
        public HomeController(IProductVariantServices productvariantServices, IHttpContextAccessor contextAccessor, IUserTaskServices userTaskServices)
        {
            _productvariantServices = productvariantServices;
            _contextAccessor = contextAccessor;
            _userTaskServices = userTaskServices;
        }

        //list of Wishlist Items
        public async Task<IActionResult> WishList()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    if (logedUser == null)
                    {
                        return RedirectToAction("Login", "Users");
                    }
                    else
                    {
                        IEnumerable<WishListVM> wishlistItemsList = await _userTaskServices.GetAllWishlistItems(logedUser.UserId);
                        if(wishlistItemsList.Count() == 0 )
                        {
                            ViewData["wishlist"] = "Seems you have no item in your wishlist yet!";
                            return View();
                        }
                        return View(wishlistItemsList);
                    }
                }

                

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Details of particular product
        public async Task<IActionResult> Product_Details_for_Customer(int id)
        {
            try
            {
                if (TempData["wishlist"]  == null)
                {
                    ViewData["wishlistItem"] = true;
                }
                else
                {
                    bool wishlist = (bool)TempData["wishlist"];
                    ViewData["wishlistItem"] = wishlist;
                }
               
                var productvariantDetails = await _userTaskServices.GetProductVariantByIds(id);
                return View(productvariantDetails);
                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //Wishlist an Item that include remove item from wishlist also
        public async Task<IActionResult> WishlistItem(int productVariantId)
        {
            try
            {

                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    if (logedUser == null)
                    {
                        return RedirectToAction("Login", "Users");
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            var wishlistItem = await _userTaskServices.Wishlist(productVariantId, logedUser.UserId);
                            TempData["wishlist"] = wishlistItem.IsDelete;
                        }
                        return RedirectToAction("Product_Details_for_Customer", new { id = productVariantId });

                    }
                }
                
               

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


       
    }
}
