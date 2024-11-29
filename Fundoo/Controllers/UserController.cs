using BusinessLayer.Interface;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System.Net;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL; 
        private readonly ICacheDL _cacheDL;
        public UserController(IUserBL userBL, ICacheDL cacheDL)
        {
            _userBL = userBL;
            _cacheDL = cacheDL;
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

        [HttpGet("Get-All-Users")]
        public async Task<ResponseModel<List<User>>> GetAllUsers()
        {
            
            string cacheKey = "AllUsers";

            // Attempt to retrieve the list of users from Redis cache
            var cachedUsers = _cacheDL.GetData<List<User>>(cacheKey);
            if (cachedUsers != null && cachedUsers.Count > 0)
            {
                return new ResponseModel<List<User>>
                {
                    Data = cachedUsers,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "Users retrieved from cache successfully",
                    Success = true
                };
            }

            // Fetch users from the database if not in cache
            var response = await _userBL.GetAllUsers();

            // Cache the result if the response is successful
            if (response.Success && response.Data != null && response.Data.Count > 0)
            {
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(15); 
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpGet("Get-User-ById")]
        public async Task<ResponseModel<User>> GetUserById(int userId)
        {
            
            string cacheKey = $"User_{userId}";

            // Attempt to retrieve the user from Redis cache
            var cachedUser = _cacheDL.GetData<User>(cacheKey);
            if (cachedUser != null)
            {
                return new ResponseModel<User>
                {
                    Data = cachedUser,
                    StatusCode = (int)HttpStatusCode.OK,
                    Message = "User retrieved from cache successfully",
                    Success = true
                };
            }

            // Fetch the user from the database if not in cache
            var response = await _userBL.GetUserById(userId);

            // Cache the result if the response is successful
            if (response.Success && response.Data != null)
            {
                var cacheExpiration = DateTimeOffset.Now.AddMinutes(15); 
                _cacheDL.SetData(cacheKey, response.Data, cacheExpiration);
            }

            return response;
        }


        [HttpDelete("Delete-User")]
        public async Task<ResponseModel<bool>> DeleteUserById(int userId)
        {
            
            var response = await _userBL.DeleteUserById(userId);

            if (response.Success)
            {
                
                string userCacheKey = $"User_{userId}";
                string allUsersCacheKey = "AllUsers";

                
                _cacheDL.RemoveData(userCacheKey);

                // Remove the cached list of all users to ensure consistency
                _cacheDL.RemoveData(allUsersCacheKey);
            }

            return response;
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
