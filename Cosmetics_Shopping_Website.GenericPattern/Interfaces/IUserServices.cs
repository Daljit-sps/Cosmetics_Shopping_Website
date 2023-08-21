using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IUserServices
    {
        Task<bool> ValidateCredentials(LoginVM logeduser);

        Task<bool> ValidateCredentialsAfterSignUp(LoginVM logeduser);

        Task<User> GetForLogin(LoginVM logeduser);

       
        Task<User> FindByEmail(string Email);

        Task<User> FindByEmailR(ResetPasswordVM objResetPasswordVM);

        Task<User> ResetPassword(User users, ResetPasswordVM objResetPasswordVM);
        Task<SignUpVM> SignUp(SignUpVM objSignUp);

        Task<IEnumerable<GetUserVM>> GetAllUsers();

        Task<GetUserVM> GetUserByIds(int id);

        Task<PutUserVM> PutUser(PutUserVM objPutUser, int logedUserId);

        Task<bool> DeleteUser(int id, int logedUserId);

    }
}
