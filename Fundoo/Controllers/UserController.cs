using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Model;
using Model.Utility;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL; 
        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> Registration([FromBody]UserRegistration userRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(userRegistration);
            }

            var responce = await _userBL.Registration(userRegistration);
            if (responce.Success)
            {
                return Ok(responce);
            }
            return Conflict(responce);
        }

        [HttpPut("UpdateUser/{id}")]
        public async Task<ResponseModel<User>> UpdateRegisterUser(int id, UserRegistration updatedUser)
        {
            return await (_userBL.UpdateRegisterUser(id, updatedUser));
        }

        [HttpPost("ValidUser")]
        public async Task<User> ValidateUser(Login login)
        {
            return await _userBL.ValidateUser(login);
        }

        [HttpPost("Token")]
        public async Task<string> Login(Login users)
        {
            return await _userBL.Login(users);
        }

        [HttpPost("Forget-Password")]
        public async Task<ResponseModel<string>> ForgotPassword(string email)
        {
            return await _userBL.ForgotPassword(email);
        }


        [HttpPost("Reset-Password")]
        public async Task<ResponseModel<string>> ResetPassword(string token, string newPassword)
        {
            return await _userBL.ResetPassword(token, newPassword);
        }
    }
}
