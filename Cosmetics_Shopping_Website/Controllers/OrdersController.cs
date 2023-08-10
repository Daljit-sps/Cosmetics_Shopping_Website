using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Services;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class OrdersController : Controller
    {
        public readonly IProductVariantServices _productvariantServices;
        public readonly IGetOrderServices _getOrderServices;
        private readonly IHttpContextAccessor _contextAccessor;


        public OrdersController(IProductVariantServices productvariantServices, IGetOrderServices getOrderServices, IHttpContextAccessor contextAccessor)
        {
            _productvariantServices = productvariantServices;
            _getOrderServices = getOrderServices;
            _contextAccessor = contextAccessor;
        }

        //list of Customer Orders 
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Order_Items_PaymentVM> orderList = await _getOrderServices.GetAllOrdersListAsync();
                if (orderList.Count() > 0)
                {
                    return View(orderList);
                }
                else
                {
                    ViewData["orders"] = "Looks like No one, has placed an order yet!";
                    return View();
                }

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //update Order Status
        public async Task<IActionResult> UpdateOrderStatus(int id)
        {
            try
            {
                //dropdown to select ORDER STATUS   
                List<string> OrderStatusOptions = new List<string>
                {
                    "Order Approved",
                    "Packed",
                    "Shipped",
                    "Delivered"
                };
                var getOrderStatus = await _getOrderServices.GetOrderDetailsByIdAsync(id);
                ViewBag.DropDownData = OrderStatusOptions;
                return View();

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, string OrderStatus)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                if (ModelState.IsValid)
                { 
                    var orderStatusUpdated = await _getOrderServices.UpdateOrderStatusAsync(
                        OrderId, OrderStatus, logedUser.UserId);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //details of particular order
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                List<string> OrderStatusOptions = new List<string>
                {
                    "Order Approved",
                    "Packed",
                    "Shipped",
                    "Delivered"
                };
                ViewBag.DropDownData = OrderStatusOptions;
                var orderDetails = await _getOrderServices.GetOrderDetailsByIdAsync(id);
                return View(orderDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //delete particular order
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                var orderDeleted = await _getOrderServices.DeleteOrder(id, logedUser.UserId);
                if (orderDeleted != true)
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
