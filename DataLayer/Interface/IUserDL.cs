using DataLayer.Context;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface IUserDL
    {
        public  Task<ResponseModel<User>> Registration(UserRegistration userRegistration);

        public  Task<ResponseModel<User>> UpdateRegisterUser(int id, UserRegistration updatedUser);

        public Task<User> ValidateUser(Login login);

        public string GenerateToken(Login user);

        public Task<string> Login(Login users);


    }
}
