using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserDL _userDL;

        public UserBL(IUserDL userDL)
        {
            _userDL = userDL;
        }
        public async Task<ResponseModel<User>> Registration(UserRegistration userRegistration)
        {
            return await _userDL.Registration(userRegistration);
        }

        public async Task<ResponseModel<User>> UpdateRegisterUser(int id, UserRegistration updatedUser)
        {
            return await (_userDL.UpdateRegisterUser(id, updatedUser));
        }

        public async Task<User> ValidateUser(Login login)
        {
            return await _userDL.ValidateUser(login);
        }


        public async Task<string> Login(Login users)
        {
            return await _userDL.Login(users);
        }

        public async Task<ResponseModel<string>> ForgotPassword(string email)
        {
            return await _userDL.ForgotPassword(email);
        }

        public async Task<ResponseModel<string>> ResetPassword(string token, string newPassword)
        {
            return await _userDL.ResetPassword(token, newPassword);
        }
    }
}
