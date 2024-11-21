using DataLayer.Context;
using DataLayer.Exceptions;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using Model.Model;
using Model.Utility;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class UserDL : IUserDL 
    {
        private readonly FundooContext _context;
        private readonly IConfiguration _configuration;

        
        public UserDL(FundooContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<ResponseModel<User>> Registration(UserRegistration userRegistration)
        {
            try
            {
                var user = _context.users.FirstOrDefault(user => user.EmailAddress.Equals(userRegistration.emailAddress));
                if (user != null)
                {
                    throw new UserException("User Already Registered");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistration.password);
                user = new User
                {
                    Name = userRegistration.name,
                    EmailAddress = userRegistration.emailAddress,
                    PhoneNumber = userRegistration.phoneNumber,
                    PasswordHash = hashedPassword
                };

                await _context.users.AddAsync(user);

            
                await _context.SaveChangesAsync();


                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User Registrated Successfully",
                    StatusCode = (int)HttpStatusCode.Created,
                    Success = true
                };
            }
            catch (UserException ex)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "User Already Registrated ",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Success = false
                };

            }
            catch (Exception ex)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "User Already Registrated ",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<User>> UpdateRegisterUser(int id, UserRegistration updatedUser)
        {
            try
            {
                
                var user = await _context.users.FindAsync(id);

                
                if (user == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                
                user.Name = updatedUser.name;
                user.PhoneNumber = updatedUser.phoneNumber;

                
                await _context.SaveChangesAsync();

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User updated successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "Failed to update user",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<User> ValidateUser(Login login)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.EmailAddress.Equals(login.emailAddress));

            if (user == null)
            {
                return null; // Return null if the user is not found
            }

            if (BCrypt.Net.BCrypt.Verify(login.password, user.PasswordHash))
            {
                return user; // Return the user object directly if the login is successful
            }
            else
            {
                return null; // Return null if the password is invalid
            }
        }

        public string GenerateToken(Login user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> Login(Login user)
        {
            var user_ = await ValidateUser(user); // Await the async UserLogin method

            var newuser = new Login
            {
                emailAddress = user.emailAddress,
                password = user.password
            };
            if (user_ != null)
            {
                try
                {
                    var token = GenerateToken(newuser);
                    return token;
                }
                catch (Exception ex)
                {
                    // Log exception (consider using a logging framework)
                    Console.WriteLine($"Token generation failed: {ex.Message}");
                }
            }

            return null;
        }

    }


}
