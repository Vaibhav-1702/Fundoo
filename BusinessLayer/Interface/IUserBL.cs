﻿using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        public Task<ResponseModel<User>> Registration(UserRegistration userRegistration);

        public Task<ResponseModel<User>> UpdateRegisterUser(int id, UserRegistration updatedUser);

        public Task<ResponseModel<List<User>>> GetAllUsers();

        public Task<ResponseModel<User>> GetUserById(int userId);

        public Task<ResponseModel<bool>> DeleteUserById(int userId);

        public Task<User> ValidateUser(Login login);

        public Task<string> Login(Login users);

        public Task<ResponseModel<string>> ForgotPassword(string email);

        public Task<ResponseModel<string>> ResetPassword(string token, string newPassword);
    }
}
