using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmetics_Shopping_Website.GenericPattern.Security;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class UserServices: IUserServices
    {
        public IGenericRepository _genericRepository;

        public UserServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<bool> ValidateCredentials(LoginVM logeduser)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.Email == logeduser.Email && x.IsDelete == false);
                if (user == null)
                {
                    return false;
                }
                return EncryptPassword.VerifyPassword(logeduser.Password, user.Password);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidateCredentialsAfterSignUp(LoginVM logeduser)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.Email == logeduser.Email && x.Password == logeduser.Password && x.IsDelete == false);
                if (user == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<User> GetForLogin(LoginVM logeduser)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.Email == logeduser.Email && x.IsDelete == false);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> FindByEmail(string Email)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.Email == Email && x.IsDelete == false);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> FindByEmailR(ResetPasswordVM objResetPasswordVM)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.Email == objResetPasswordVM.Email && x.IsDelete == false);
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        
        public async Task<User> ResetPassword(User users,ResetPasswordVM objResetPasswordVM)
        {
            try
            {
                var user = await _genericRepository.Get<User>(x => x.UserId == users.UserId);
                if (user != null)
                {
                    user.Password = objResetPasswordVM.ConfirmPassword;
                    await _genericRepository.Put(user);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<GetUserVM>> GetAllUsers()
        {
            try
            {
                var userDetailsList = await _genericRepository.GetAll<User>();
                return userDetailsList.Select(e => new GetUserVM
                {
                    UserId = e.UserId,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Password = e.Password,
                    MobileNumber = e.MobileNumber,
                    RoleId = e.RoleId,
                    IsDelete = e.IsDelete,
                }).Where(e=>e.RoleId == 3 && e.IsDelete == false).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }



        public async Task<GetUserVM> GetUserByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var userDetails = await _genericRepository.GetById<User>(id);
                    if (userDetails != null && userDetails.IsDelete == false)
                    {
                        GetUserVM result = JsonConvert.DeserializeObject<GetUserVM>(JsonConvert.SerializeObject(userDetails))!;
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }
        public async Task<SignUpVM> SignUp(SignUpVM objSignUp)
        {
            try
            {
                var getmail = await _genericRepository.Get<User>(e=>e.Email ==  objSignUp.Email && e.IsDelete == false);
                if(getmail != null)
                {
                    return null;
                }
                else
                {
                    User objUser = new();
                    if (objSignUp != null)
                    {
                        objUser.FirstName = objSignUp.FirstName;
                        objUser.LastName = objSignUp.LastName;
                        objUser.MobileNumber = objSignUp.MobileNumber;
                        objUser.Email = objSignUp.Email;
                        objUser.Password = objSignUp.Password;
                        objUser.CreatedOn = DateTime.Now;
                        objUser.UpdatedOn = DateTime.Now;
                        objUser.CreatedBy = objSignUp.UserId;
                        objUser.UpdatedBy = objSignUp.UserId;
                        await _genericRepository.Post(objUser);
                        SignUpVM result = JsonConvert.DeserializeObject<SignUpVM>(JsonConvert.SerializeObject(objUser))!;
                        return result;
                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
            return null;

        }

        public async Task<PutUserVM> PutUser(PutUserVM objPutUser, int logedUserId)
        {
            try
            {
                User objUser = await _genericRepository.GetById<User>(objPutUser.UserId);
                if (objUser != null && objUser.IsDelete == false)
                {
                    objUser.FirstName = objPutUser.FirstName;
                    objUser.LastName = objPutUser.LastName;
                    objUser.MobileNumber = objPutUser.MobileNumber;
                    objUser.Email = objPutUser.Email;
                    objUser.UpdatedOn = DateTime.Now;
                    objUser.UpdatedBy = logedUserId;

                    _genericRepository.Put(objUser);
                    PutUserVM result = JsonConvert.DeserializeObject<PutUserVM>(JsonConvert.SerializeObject(objUser))!;
                    return result;

                }

            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<bool> DeleteUser(int id, int logedUserId)
        {
            if (id > 0)
            {
                var userDetails = await _genericRepository.GetById<User>(id);
                if (userDetails != null)
                {
                    userDetails.IsDelete = true;
                    userDetails.UpdatedBy = logedUserId;
                    userDetails.UpdatedOn = DateTime.Now;
                    var result = _genericRepository.Save();

                    
                    if (result > 0)
                    {
                        return true;
                    }

                }
            }
            return false;
        }

    }
}
