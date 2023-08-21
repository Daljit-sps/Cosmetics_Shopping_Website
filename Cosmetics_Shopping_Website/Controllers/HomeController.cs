using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Newtonsoft.Json;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;
using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;
using Stripe;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Cosmetics_Shopping_Website.GenericPattern.StaticDetails_SD;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using crypto;
using System.Net;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class HomeController : Controller
    {
        public readonly IProductVariantServices _productvariantServices;
        private readonly StripeSettings _stripeSettings;
        public readonly IEmailServices _emailService;
        public readonly IViewRenderService _viewRenderService;
        public readonly IUserTaskServices _userTaskServices;
        public readonly IPlacingOrderServices _placingOrderServices;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;

        public HomeController(IOptions<StripeSettings> stripeOptions,IEmailServices emailServices,IViewRenderService viewRenderService,
            IProductVariantServices productvariantServices, IHttpContextAccessor contextAccessor, 
            IUserTaskServices userTaskServices, IPlacingOrderServices placingOrderServices, IConfiguration configuration)
        {
            _stripeSettings = stripeOptions.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _emailService = emailServices;
            _viewRenderService = viewRenderService;
            _productvariantServices = productvariantServices;
            _contextAccessor = contextAccessor;
            _userTaskServices = userTaskServices;
            _placingOrderServices = placingOrderServices;
            _configuration = configuration;
        }

        //Search
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchText)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (!string.IsNullOrEmpty(objUser))
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    ViewData["IsLogin"] = "Yes";
                    IEnumerable<ProductVariantVM> getSearchResult = await _userTaskServices.SearchVariantsForLogedUser(searchText, logedUser.UserId);

                    if (getSearchResult.Count() > 0)
                    {
                        ViewBag.SearchText = searchText;
                        return View(getSearchResult);
                    }
                    else
                    {
                        ViewBag.SearchMsg = "Sorry, there is no such item present with this name";
                        return View();
                    }
                }
                else
                {
                    IEnumerable<ProductVariantVM> getSearchResult = await _userTaskServices.SearchVariants(searchText);

                    if (getSearchResult.Count() > 0)
                    {
                        ViewBag.SearchText = searchText;
                        return View(getSearchResult);
                    }
                    else
                    {
                        ViewBag.SearchMsg = "Sorry, there is no such item present with this name";
                        return View();
                    }
                }
                
                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //list of Wishlist Items
        public async Task<IActionResult> WishList()
        {
            try
            {

                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");

                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                ViewData["IsLogin"] = "Yes";
                IEnumerable<WishListVM> wishlistItemsList = await _userTaskServices.GetAllWishlistItems(logedUser.UserId);
                if (wishlistItemsList.Count() == 0)
                {
                    ViewData["wishlist"] = "Seems you have no item in your wishlist yet!";
                    return View();
                }
                return View(wishlistItemsList);
  


            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //list of Cart Items
        public async Task<IActionResult> Cart()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                IEnumerable<CartItemsVM> cartItemsList = await _userTaskServices.GetAllCartItems(logedUser.UserId);

                if (cartItemsList.Count() == 0)
                {
                    ViewData["IsLogin"] = "Yes";
                    ViewData["cart"] = "Your cart is empty, get started with shopping by clicking on below button";
                    return View();
                }
                else
                {
                    ViewData["IsLogin"] = "Yes";
                    ViewData["CartItemsCount"] = cartItemsList.Count();
                    ViewData["TotalPrice"] = cartItemsList.Sum(x => x.Price * x.Quantity);
                    return View(cartItemsList);
                }

               



            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Wishlist an Item 
        public async Task<IActionResult> WishlistItem(int productVariantId)
        {
            try
            {

                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return Json(new { success = false });
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    if (logedUser == null)
                    {
                        return Json(new { success = false });
                    }
                    else
                    {
                        var wishlistItem = await _userTaskServices.Wishlist(productVariantId, logedUser.UserId);
                        if (wishlistItem.IsDelete)
                        {
                            return Json(new { success = false });
                        }
                        return Json(new { success = true });
                        
                    }
                }



            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Details of particular product
        [AllowAnonymous]
        public async Task<IActionResult> Product_Details_for_Customer(int id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    var productvariantDetails = await _userTaskServices.GetProductVariantById(id);
                    //get similar product variants
                    IEnumerable<ProductVariantVM> getSimilarProductVariants = await _userTaskServices.GetSimilarProductVariants(productvariantDetails.ProductId, productvariantDetails.Id);
                    if (getSimilarProductVariants.Count() > 0)
                    {
                        ViewBag.VariantDetails = productvariantDetails;
                        ViewBag.HasSimilarItems = "yes";
                        return View(getSimilarProductVariants);
                       
                    }
                    else
                    {
                        ViewBag.VariantDetails = productvariantDetails;
                        return View();
                    }
                    
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    ViewData["IsLogin"] = "Yes";
                    var productvariantDetails = await _userTaskServices.GetProductVariantByIdForLogedUser(id, logedUser.UserId);
                    //get similar product variants
                    IEnumerable<ProductVariantVM> getSimilarProductVariants = await _userTaskServices.GetSimilarProductVariants(productvariantDetails.ProductId, productvariantDetails.Id);
                    if (getSimilarProductVariants.Count() > 0)
                    {
                        ViewBag.VariantDetails = productvariantDetails;
                        ViewBag.HasSimilarItems = "yes";
                        return View(getSimilarProductVariants);

                    }
                    else
                    {
                        ViewBag.VariantDetails = productvariantDetails;
                        return View();
                    }
                    
                }
               

            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //Change Quantity of Cart Item
        public async Task<IActionResult> CartItemQuantityChange(int cartItemId, int quantity)
        {
            try
            {

                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (cartItemId != 0 && quantity != 0)
                {
                    var editCartItem = await _userTaskServices.UpdateCartItemQuantity(cartItemId, quantity, logedUser.UserId);
                    return Json(editCartItem);
                }
                else
                {
                    return RedirectToAction(nameof(Cart));
                }

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Add to Cart
        public async Task<IActionResult> AddToCart(int productVariantId, int quantity)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return Json(new { success = false, message = "Kindly, Login or SignUp" });
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    if (logedUser == null)
                    {
                        return Json(new { success = false, message = "Kindly, Login or SignUp" });
                       
                    }
                    else
                    {
                        var addToCartItem = await _userTaskServices.AddToCart(productVariantId, quantity, logedUser.UserId);
                        if(addToCartItem == null)
                        {
                            return Json(new { success = false, message = "Selected Item already exist in your cart" });
                            //return RedirectToAction("Product_Details_for_Customer", new { id = productVariantId });
                        }
                        else
                        {
                            return Json(new { success = true });
                            
                        }
                       
                    }
                }



            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred." });
            }
        }


        //Remove from Cart
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (cartItemId != 0)
                {
                    var removeCartItem = await _userTaskServices.RemoveFromCart(cartItemId, logedUser.UserId);
                    return Json(removeCartItem);
                }
                else
                {
                    return RedirectToAction(nameof(Cart));
                }
                

            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //list of ProductVariants based on products or category or sub-category
        [AllowAnonymous]
        public async Task<IActionResult> ProductLists(string condition, int? page, int pageSize = 8)
        {
            try
            {
                int pageIndex = page ?? 1;
                IEnumerable<ProductVariantVM> productVariantsProducts = await _productvariantServices.GetAllProductVariantsBasedOnProducts(condition);
                IEnumerable<ProductVariantVM> productVariantsSC = await _productvariantServices.GetAllProductVariantsBasedOnSubCategoryOrCategory(condition);

                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (!string.IsNullOrEmpty(objUser))
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                    ViewData["IsLogin"] = "Yes";
                    IEnumerable<ProductVariantVM> productvariantsList = await _userTaskServices.GetAllProductVariantsBasedOnProductWithPaginationForLogedUser(logedUser.UserId,condition, pageIndex, pageSize);
                    IEnumerable<ProductVariantVM> productvariantsListBasedOnSubCategoryOrCategory = await _userTaskServices.GetAllProductVariantsBasedOnSubCategoryOrCategoryWithPaginationForLogedUser(logedUser.UserId,condition, pageIndex, pageSize);
                    if (productvariantsList.Count() > 0)
                    {
                        var result = new ProductListViewModel
                        {
                            Products = productvariantsList,
                            TotalCount = productVariantsProducts.Count(),
                            PageSize = pageSize,
                            CurrentPage = pageIndex
                        };
                        ViewBag.ProductName = condition;
                        return View(result);
                    }
                    else
                    {
                        var result = new ProductListViewModel
                        {
                            Products = productvariantsListBasedOnSubCategoryOrCategory,
                            TotalCount = productVariantsSC.Count(),
                            PageSize = pageSize,
                            CurrentPage = pageIndex
                        };

                        ViewBag.Name = condition;
                        return View(result);
                    }
                }
                else
                {
                    IEnumerable<ProductVariantVM> productvariantsList = await _productvariantServices.GetAllProductVariantsBasedOnProductWithPagination(condition, pageIndex, pageSize);
                    IEnumerable<ProductVariantVM> productvariantsListBasedOnSubCategoryOrCategory = await _productvariantServices.GetAllProductVariantsBasedOnSubCategoryOrCategoryWithPagination(condition, pageIndex, pageSize);
                    if (productvariantsList.Count() > 0)
                    {
                        var result = new ProductListViewModel
                        {
                            Products = productvariantsList,
                            TotalCount = productVariantsProducts.Count(),
                            PageSize = pageSize,
                            CurrentPage = pageIndex
                        };
                        ViewBag.ProductName = condition;
                        return View(result);
                    }
                    else
                    {
                        var result = new ProductListViewModel
                        {
                            Products = productvariantsListBasedOnSubCategoryOrCategory,
                            TotalCount = productVariantsSC.Count(),
                            PageSize = pageSize,
                            CurrentPage = pageIndex
                        };

                        ViewBag.Name = condition;
                        return View(result);
                    }
                }
               
               
               
                
                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        
        //Shipping Address
        public async Task<IActionResult> UserShippingAddress()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);


                //dropdown to select State
                var selectListItemsForStates = await _userTaskServices.GetStates();
                var selectListForStates = new SelectList(selectListItemsForStates, "Id", "StateName");
                ViewBag.DropDownDataForStates = selectListForStates;

                //get country
                var country = await _userTaskServices.GetCountry();
                ViewData["Country"] = country.CountryName;
                ViewData["IsLogin"] = "Yes";
                //user details
                var logeduserDetails = await _userTaskServices.GetLogedUserDetails(logedUser.UserId);
                if (logeduserDetails != null)
                {
                    return View(logeduserDetails);
                }
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //Add User Shipping Address
        [HttpPost]
        public async Task<IActionResult> UserShippingAddress(UserAddressVM objUserShippingAddress)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if(ModelState.IsValid)
                {
                    var shippingAddress = await _userTaskServices.AddUserShippingAddress(objUserShippingAddress, logedUser.UserId);
                    if(shippingAddress == null)
                    {
                        ViewData["AddressExistMsg"] = "This Address already exist in your addresses";
                    }
                    else
                    {
                        ViewData["AddressAddedMsg"] = "Your Address has been added";
                    }

                    return RedirectToAction(nameof(SelectDeliveryAddress)); 
                }
                else
                {
                   
                    return View();
                }
               
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //Edit User Shipping Address
        public async Task<IActionResult> EditUserShippingAddress(int Id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                //dropdown to select State
                var selectListItemsForStates = await _userTaskServices.GetStates();
                var selectListForStates = new SelectList(selectListItemsForStates, "Id", "StateName");
                ViewBag.DropDownDataForStates = selectListForStates;

                //get country
                var country = await _userTaskServices.GetCountry();
                ViewData["Country"] = country.CountryName;
                //user details and user address details
                var logeduserAddressDetails = await _userTaskServices.GetUserShippingAddressById(Id, logedUser.UserId);
                if (logeduserAddressDetails != null)
                {
                    ViewData["IsLogin"] = "Yes";
                    return View(logeduserAddressDetails);
                    /*var jsonData = JsonConvert.SerializeObject(logeduserAddressDetails);
                    return Json(jsonData);*/
                }
                else
                {
                    return RedirectToAction(nameof(CheckoutDetails));
                }
               

                    
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUserShippingAddress(UserAddressVM objUserShippingAddress)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                //get country
                var country = await _userTaskServices.GetCountry();
                
                if (ModelState.IsValid)
                {
                    var shippingAddress = await _userTaskServices.EditUserShippingAddress(objUserShippingAddress, logedUser.UserId);
                    ViewData["SuccessMsg"]= "Address has been successfully edited!";
                    ViewData["Country"] = country.CountryName;
                    return View(shippingAddress);

                }
                return RedirectToAction(nameof(EditUserShippingAddress));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Delete user address
        public async Task<IActionResult> DeleteUserShippingAddress(int addressId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (addressId >0)
                {
                    var deleteUserAddress = await _userTaskServices.DeleteUserAddress(addressId, logedUser.UserId);
                    return Json(deleteUserAddress);
                } 
                else
                {
                    return RedirectToAction(nameof(SelectDeliveryAddress)); ;
                }
               
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Select Delivery Address
        public async Task<IActionResult> SelectDeliveryAddress(int productVariantId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return Json(new { success = false });
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                    if (productVariantId > 0)
                    {
                        TempData["ProductVariantIdForBuyNow"] = productVariantId;

                    }

                    //get list of user addresses
                    IEnumerable<UserAddressVM> getUserAddress = await _userTaskServices.GetUserAddresses(logedUser.UserId);
                    if (getUserAddress.Count() > 0)
                    {

                        ViewData["IsLogin"] = "Yes";
                        ViewData["UserAdressesCount"] = getUserAddress.Count();
                        return View(getUserAddress);
                    }
                    else
                    {
                        ViewData["IsLogin"] = "Yes";
                        return RedirectToAction(nameof(UserShippingAddress));
                    }
                }
               
               
            }
            catch(Exception)
            {
                return View("Error");
            }
        }

        //Get Selected Delivery Address
        public async Task<IActionResult> GetSelectedDeliveryAddress(int selectedAddressId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                if (selectedAddressId > 0)
                {
                    TempData["SelectedAddress"] = selectedAddressId;
                    return RedirectToAction(nameof(CheckoutDetails));
                    
                    
                }
                else
                {
                    return RedirectToAction("SelectDeliveryAddress");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Checkout Details
        public async Task<IActionResult> CheckoutDetails()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                decimal shippingPrice = 250;
                int pvForBuyNow = 0;
                ViewData["ShippingPrice"] = shippingPrice;
                ViewData["UserName"] = logedUser.FirstName + " " + logedUser.LastName;
                ViewData["IsLogin"] = "Yes";
                if (TempData.ContainsKey("ProductVariantIdForBuyNow") && TempData["ProductVariantIdForBuyNow"] is int tempValue)
                {
                    pvForBuyNow = tempValue;
                    var getProductVariant = await _productvariantServices.GetProductVariantByIds(pvForBuyNow);
                    ViewData["SubTotalPrice"] = getProductVariant.Price;
                    ViewData["TotalPrice"] = getProductVariant.Price + shippingPrice;
                    ViewBag.ProductVariant = getProductVariant;

                    return View();
                }
                else
                {
                    //get list of cart items
                    IEnumerable<CartItemsVM> cartItemsList = await _userTaskServices.GetAllCartItems(logedUser.UserId);
                    ViewData["CartItemsCount"] = cartItemsList.Count();
                    ViewData["SubTotalPrice"] = cartItemsList.Sum(x => x.Price * x.Quantity);
                    ViewData["TotalPrice"] = cartItemsList.Sum(x => x.Price * x.Quantity) + shippingPrice;
                    ViewData["UserName"] = logedUser.FirstName + " " + logedUser.LastName;
                    ViewData["IsLogin"] = "Yes";

                    return View(cartItemsList);

                }
               
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        //Payment
        public async Task<IActionResult> Checkout(string data)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                //get required objects
                decimal shippingPrice = 250;
                int userAddressId = Convert.ToInt32(TempData["SelectedAddress"]);
               
                
                if(data != null)
                {
                    var productVariant = JsonConvert.DeserializeObject<ProductVariantVM>(data);
                    var variantsList = new List<CartItemsVM>
                    {
                        new CartItemsVM
                        {
                             ProductVariantId = productVariant.Id,
                             Quantity = 1,
                             Price = productVariant.Price,
                             VariantName = productVariant.VariantName
                        }

                    };
                    decimal totalPrice = productVariant.Price + shippingPrice;
                    var order = await _placingOrderServices.CreateOrderAsync(totalPrice, logedUser.UserId);
                    var orderItems = await _placingOrderServices.CreateOrderItemsAsync(order.Id, variantsList, userAddressId, logedUser.UserId);

                    //payment section
                    //var domain = "http://182.75.88.147:8145/";
                    //var domain = "https://glancecosmetics.softprodigy.in";
                    // domain = "https://localhost:7077/";
                    var domain = "https://glancecosmetic.softprodigy.in/";
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = domain + $"Home/OrderConfirmation?id={order.Id}",
                        //SuccessUrl = domain + $"Users/HomePage",
                        CancelUrl = domain + $"Home/CheckoutDetails",
                    };

                    foreach (var item in variantsList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (int)(item.Price * 100),
                                Currency = "INR",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.VariantName
                                },

                            },
                            Quantity = 1
                        };
                        options.LineItems.Add(sessionLineItem);
                    }
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (int)(shippingPrice * 100),
                            Currency = "INR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Shipping Cost",

                            },

                        },
                        Quantity = 1,
                    });
                    
                    var service = new SessionService();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    Session session = service.Create(options);
                    _placingOrderServices.UpdateOrderStripePaymentIdAsync(order.Id, session.Id);
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
               
                else
                {
                    IEnumerable<CartItemsVM> cartItemsList = await _userTaskServices.GetAllCartItems(logedUser.UserId);
                    IEnumerable<CartItemsVM> variantsList = await _userTaskServices.GetVariantsListPresentInCart(cartItemsList);
                    decimal totalPrice = cartItemsList.Sum(x => x.Price * x.Quantity) + shippingPrice;


                    var order = await _placingOrderServices.CreateOrderAsync(totalPrice, logedUser.UserId);
                    var orderItems = await _placingOrderServices.CreateOrderItemsAsync(order.Id, variantsList, userAddressId, logedUser.UserId);

                    //payment section
                    
                    //var domain = "http://182.75.88.147:8145/";
                    var domain = "https://glancecosmetic.softprodigy.in/";
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                        SuccessUrl = domain + $"Home/OrderConfirmation?id={order.Id}",
                        //SuccessUrl = domain + $"Users/HomePage",
                        CancelUrl = domain + $"Home/CheckoutDetails",
                    };

                    foreach (var item in cartItemsList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (int)(item.Price * 100),
                                Currency = "INR",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.VariantName
                                },

                            },
                            Quantity = item.Quantity
                        };
                        options.LineItems.Add(sessionLineItem);
                    }
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (int)(shippingPrice * 100),
                            Currency = "INR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Shipping Cost",

                            },

                        },
                        Quantity = 1,
                    });
                   
                    
                    var service = new SessionService();
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                    Session session = service.Create(options);  
                    _placingOrderServices.UpdateOrderStripePaymentIdAsync(order.Id, session.Id);
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }

                
                
            }
            catch(StripeException ex)
            {
                ErrorViewModel errorModel = new();

                errorModel.ErrorMessage = "Stripe Exception >> " + ex.Message;
                errorModel.StackTrace = ex.StackTrace;
                return View("Error", errorModel);
            }
            catch (Exception ex)
            {
                ErrorViewModel errorModel = new();

                errorModel.ErrorMessage = ex.Message;
                errorModel.StackTrace = ex.StackTrace;
                while (ex.InnerException != null)
                {
                    errorModel.ErrorMessage = ex.InnerException.Message;
                    errorModel.StackTrace = ex.InnerException.StackTrace;
                    ex= ex.InnerException;
                }
                return View("Error", errorModel);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                Order order = await _placingOrderServices.GetOrderByIdAsync(id);
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                //check stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _placingOrderServices.UpdateOrderStatusAsync(order.Id, StaticDetails.OrderStatusApproved, StaticDetails.PaymentStatusPaid, session.PaymentIntentId);
                }

                // order placed email
                var getOrderOverView = await _placingOrderServices.GetOrderDetailsByIdAsync(id, logedUser.FirstName, logedUser.LastName);

                string emailContent = await _viewRenderService.RenderToStringAsync("Home/OrderPlacedEmail", getOrderOverView);
                var message = new Message(new string[] { logedUser.Email }, "Order Confirmation!!!", emailContent);
                _emailService.SendEmail(message);

                await _userTaskServices.RemoveAllItemsFromCart(logedUser.UserId);

                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //list of Customer Orders 
        public async Task<IActionResult> UserOrders()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (logedUser != null)
                {
                    ViewData["IsLogin"] = "Yes";
                }

                IEnumerable<Order_Items_PaymentVM> orderList = await _placingOrderServices.GetUserOrdersListAsync(logedUser.UserId);
                if(orderList.Count()>0)
                {
                   
                    return View(orderList);
                }
                else
                {
                    ViewData["orders"] = "Looks like you haven't placed any order yet!";
                    return View();
                }
               
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //order details
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                var getOrderDetails = await _placingOrderServices.GetOrderDetailsByIdAsync(id, logedUser.FirstName, logedUser.LastName);
                IEnumerable<Order_Items_PaymentVM> getOrderItemsList = await _placingOrderServices.GetOrderItemsListOfUserAsync(id);
                if(getOrderDetails != null)
                {
                    ViewBag.OrderItemsList = getOrderItemsList;
                    return View(getOrderDetails);
                }
                else
                {
                    return null;
                }
            }
            catch(Exception)
            {
                return View("Error");
            }
        }

        //add address in profile
        [HttpPost]
        public async Task<IActionResult> UserShippingAddressProfile(UserAddressVM objUserShippingAddress)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                if (ModelState.IsValid)
                {
                    var shippingAddress = await _userTaskServices.AddUserShippingAddress(objUserShippingAddress, logedUser.UserId);
                    return RedirectToAction("Profile", "Users");
                }
                else
                {
                    return RedirectToAction("Profile", "Users");
                }

            }
            catch (Exception)
            {
                return View("Error");
            }
        }
    }
}
