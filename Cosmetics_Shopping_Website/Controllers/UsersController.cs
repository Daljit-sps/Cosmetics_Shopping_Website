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

namespace Cosmetics_Shopping_Website.Controllers
{
    public class UsersController : Controller
    {
        public readonly IUserServices _userService;
        public readonly IEmailServices _emailService;
        public readonly IProductVariantServices _productvariantServices;
        private readonly IHttpContextAccessor _contextAccessor;


        public UsersController(IUserServices userService, IProductVariantServices productVariantServices ,IHttpContextAccessor contextAccessor, IEmailServices emailService)
        {
            _userService = userService;
            _productvariantServices = productVariantServices;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
        }

        //LogIn
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
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
        public async Task<IActionResult> ForgetPassword(string Email)
        {
            if (!ModelState.IsValid)
                return View();
            
            var user = await _userService.FindByEmail(Email);
            if (user == null)
                return RedirectToAction(nameof(ForgetPassword));
            
            //generating new token to reset password
            //and send reset password email
            var token = Guid.NewGuid().ToString();
            var callback = Url.Action(nameof(ResetPassword), "Users", new { token, email = user.Email }, Request.Scheme);
            var message = new Message(new string[] { user.Email }, "Reset password token", callback);
            _emailService.SendEmail(message);
            return RedirectToAction(nameof(ResetPassword));
        }

        //ResetPassword
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
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
            return RedirectToAction(nameof(ResetPassword));
        }

        //SignUp
        public async Task<IActionResult> SignUp()
        {
            return View();
        }

        [HttpPost]
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
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                return View(objSignUp);

            }
            catch (Exception)
            {
                return View("Error");
            }
            
        }

        //  Dashboard
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

                            return View();
                        }
                        else
                        {

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

        //Index
        public async Task<IActionResult> HomePage()
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

        //Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var usersDetailsList = await _userService.GetAllUsers();
                return View(usersDetailsList);

                /*var objUser = _contextAccessor.HttpContext.Session.GetString("UserData");
                var logedUser = JsonConvert.DeserializeObject<User>(objUser);
                
                if (logedUser == null)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    if (logedUser.RoleId == 2)
                    {
                        var usersDetailsList = await _userService.GetAllUsers();
                        return View(usersDetailsList);
                    }
                    else
                    {
                        var usersDetailsList = await _userService.GetAllUsers();
                        return View(usersDetailsList);
                    }

                }*/



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
                return View(userDetails);
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
                var userDetails = await _userService.GetUserByIds(id);
                var result = new PutUserVM()
                {
                    UserId = userDetails.UserId,
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email,
                    Password = userDetails.Password,
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
                    objPutUserVM.Password = EncryptPassword.TextToEncrypt(objPutUserVM.Password);
                    var userUpdated = await _userService.PutUser(objPutUserVM, logedUser.UserId);
                    return RedirectToAction(nameof(Index));
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

        //Delete
       
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

       
    }
   
}
