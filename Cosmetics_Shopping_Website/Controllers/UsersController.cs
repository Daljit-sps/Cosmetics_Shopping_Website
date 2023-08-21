using Microsoft.AspNetCore.Mvc;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System.Text;
using Newtonsoft.Json;
using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.EmailConfig;
using Cosmetics_Shopping_Website.GenericPattern.Services;
using Microsoft.AspNetCore.Authorization;
using Cosmetics_Shopping_Website.GenericPattern.Security;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cosmetics_Shopping_Website.Controllers
{
    public class UsersController : Controller
    {
        public readonly IUserServices _userService;
        public readonly IEmailServices _emailService;
        public readonly ICategoryServices _categoryServices;
        public readonly ISubCategoryServices _subCategoryServices;
        public readonly IProductServices _productServices;
        public readonly IProductVariantServices _productvariantServices;
        public readonly IUserTaskServices _userTaskServices;
        public readonly IGetOrderServices _getOrderServices;
        private readonly IHttpContextAccessor _contextAccessor;


        public UsersController(IUserServices userService, IProductVariantServices productVariantServices ,IHttpContextAccessor contextAccessor, IEmailServices emailService,
            ICategoryServices categoryServices, ISubCategoryServices subCategoryServices,IProductServices productServices,
            IUserTaskServices userTaskServices, IGetOrderServices getOrderServices)
        {
            _userService = userService;
            _productvariantServices = productVariantServices;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
            _categoryServices = categoryServices; 
            _subCategoryServices = subCategoryServices;
            _productServices = productServices;
            _userTaskServices = userTaskServices;
            _getOrderServices = getOrderServices;
        }

        //LogIn
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Login(LoginVM objLoginVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await _userService.GetForLogin(objLoginVM);
                    if (await _userService.ValidateCredentials(objLoginVM))
                    {
                        var stringUser = JsonConvert.SerializeObject(currentUser);
                        _contextAccessor.HttpContext.Session.SetString("UserData", stringUser);
                        return RedirectToAction(nameof(Dashboard));
                    }
                    else
                    {
                        ViewData["LoginMsg"] = "Invalid Email and Password";
                        return View();
                    }
                }
                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        
        //LogOut
        public async Task<IActionResult> LogOut()
        {
            try
            {
                HttpContext.Session.Remove("Email");
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(HomePage));
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        //ForgetPassword
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            if (Email == null)
                return View("Login");
            
            var user = await _userService.FindByEmail(Email);
            if (user == null)
                return RedirectToAction(nameof(ForgetPassword));
            
            //generating new token to reset password
            //and send reset password email
            var token = Guid.NewGuid().ToString();
            var callback = Url.Action(nameof(ResetPassword), "Users", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Reset password link", callback);
            _emailService.SendEmail(message);
            ViewData["ResetMailMsg"] = "Reset Password Mail has been sent to your mail id, kindly check and click on the present link to change the password";
            return View("Login");
        }

        //ResetPassword
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM objResetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(objResetPasswordVM);
            var user = await _userService.FindByEmailR(objResetPasswordVM);
            
            if (user == null)
                RedirectToAction(nameof(ResetPassword));
            
            //password encrypt    
            objResetPasswordVM.ConfirmPassword = EncryptPassword.TextToEncrypt(objResetPasswordVM.ConfirmPassword);
            
            //reset password
            var resetPassResult = await _userService.ResetPassword(user, objResetPasswordVM);
            if (resetPassResult == null)
            {
                return View("Error");
            }
            else
            {
                ViewData["SuccessPasswordReset"] = "Your Password has been reset, Kindly login!";
                return RedirectToAction(nameof(Login));
            }
            
        }

        //SignUp
        [AllowAnonymous]
        public async Task<IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<SignUpVM>> SignUp(SignUpVM objSignUp)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    //password Encrypt    
                    objSignUp.Password = EncryptPassword.TextToEncrypt(objSignUp.Password);
                    
                    var userCreated = await _userService.SignUp(objSignUp);
                    if (userCreated != null)
                    {
                        //successful signup email sending
                        var message = new Message(new string[] { objSignUp.Email }, "Welcome to Glance!!!", "Welcome to our website, hope you will shop from us");
                        _emailService.SendEmail(message);
                        LoginVM objLoginDetails = new()
                        {
                            Email = userCreated.Email, 
                            Password = userCreated.Password

                        };
                        var currentUser = await _userService.GetForLogin(objLoginDetails);
                        if (await _userService.ValidateCredentialsAfterSignUp(objLoginDetails))
                        {
                            var stringUser = JsonConvert.SerializeObject(currentUser);
                            _contextAccessor.HttpContext.Session.SetString("UserData", stringUser);
                            
                        }
                        return RedirectToAction(nameof(HomePage));
                    }
                    else
                    {
                        ViewData["Message"] = "Sorry!, user with this email already exist.";
                        return View();
                    }
                }
                return View(objSignUp);

            }
            catch (Exception)
            {
                return View("Error");
            }
            
        }

        //Dashboard
        [AllowAnonymous]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
               
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (string.IsNullOrEmpty(objUser))
                {
                    return RedirectToAction(nameof(HomePage));
                }
                else
                {
                    var logedUser = JsonConvert.DeserializeObject<User>(objUser);

                    if (logedUser == null)
                    {
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        if (logedUser.RoleId == 2)
                        {
                            //get lists to show the count
                            IEnumerable<Category> categoryList = await _categoryServices.GetAllCategories();
                            IEnumerable<GetUserVM> usersList = await _userService.GetAllUsers();
                            IEnumerable<Order_Items_PaymentVM> ordersList = await _getOrderServices.GetAllOrdersListAsync();
                            IEnumerable<ProductVariantVM> productVariantList = await _productvariantServices.GetAllProductVariants();
                            ViewData["CategoryCount"] = categoryList.Count();
                            ViewData["UserCount"] = usersList.Count();
                            ViewData["OrdersCount"] = ordersList.Count();
                            ViewData["ProductVariantCount"] = productVariantList.Count();
                            return View();
                        }
                        else
                        {
                            IEnumerable<WishListVM> wishlistcount = await _userTaskServices.GetAllWishlistItems(logedUser.UserId);
                            ViewData["WishlistCount"]= wishlistcount.Count();
                            return RedirectToAction(nameof(HomePage));
                        }

                    }
                }
               



            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        //Homepage
        [AllowAnonymous]
        public async Task<IActionResult> HomePage()
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                if (!string.IsNullOrEmpty(objUser))
                {
                    ViewData["IsLogin"] = "Yes";
                }
                //get productvariant list
                var productvariantDetailsList = await _productvariantServices.GetAllProductVariants();
                
                return View(productvariantDetailsList.Take(10));
            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        //User Index in Admin Panel
        public async Task<IActionResult> Index()
        {
            try
            {
                var usersDetailsList = await _userService.GetAllUsers();
                return View(usersDetailsList);

            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        //Details of particular user
         public async Task<IActionResult> Details(int id)
         {
            try
            {
                var userDetails = await _userService.GetUserByIds(id);
                var userAddresses = await _userTaskServices.GetUserAddresses(id);
                ViewBag.UserAddresses = userAddresses;
                return View(userDetails);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        //Edit User
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userDetails = await _userService.GetUserByIds(id);
                var result = new PutUserVM()
                {
                    RoleId = userDetails.RoleId,
                    UserId = userDetails.UserId,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    MobileNumber = userDetails.MobileNumber
                };

                return View(result);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(PutUserVM objPutUserVM)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (ModelState.IsValid)
                {
                    //password Encrypt    
                    //objPutUserVM.Password = EncryptPassword.TextToEncrypt(objPutUserVM.Password);
                    var userUpdated = await _userService.PutUser(objPutUserVM, logedUser.UserId);
                    return RedirectToAction(nameof(Profile));
                }
                else
                {
                    return View(objPutUserVM);
                }
            }
            catch (Exception ex)
            {
                return View(ex);
            }

        }

        //Delete User
        public async Task<IActionResult> DeleteConfirmed(int UserId)
        {
            try
            {
                var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                
                var userDeleted = await _userService.DeleteUser(UserId, logedUser.UserId);
                if(userDeleted != true)
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

        //Profile

        public async Task<IActionResult> Profile()
        {
            var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
            var logedUser = JsonConvert.DeserializeObject<User>(objUser);

            //get loged user details
            var userDetails = await _userService.GetUserByIds(logedUser.UserId);
            
            if(userDetails.RoleId == 2)
            {
                ViewBag.AdminDetails = userDetails;
                return View("AdminProfile");
            }
            else
            {
                //get list of loged user addresses
                var getUserAddresses = await _userTaskServices.GetUserAddresses(logedUser.UserId);
                
                //dropdown to select State
                var selectListItemsForStates = await _userTaskServices.GetStates();
                var selectListForStates = new SelectList(selectListItemsForStates, "Id", "StateName");
                ViewBag.DropDownDataForStates = selectListForStates;

                //get country
                var country = await _userTaskServices.GetCountry();
                ViewData["Country"] = country.CountryName;
                ViewData["IsLogin"] = "Yes";
                ViewBag.UserAddressList = getUserAddresses;
                return View(userDetails);
            }
            
        }
    }
   
}
