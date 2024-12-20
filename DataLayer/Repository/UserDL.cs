﻿using DataLayer.Context;
using DataLayer.Exceptions;
using DataLayer.Interface;
using DataLayer.UtilityClass;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Model.DTO;
using Model.Model;
using Model.Utility;
using MailKit.Net.Smtp;
using MailKit.Security;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DataLayer.Repository
{
    public class UserDL : IUserDL 
    {
        private readonly FundooContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenUtility _tokenUtility;
        private readonly ILogger<UserDL> _logger;
        private readonly RabbitMQPublisher _rabbitMQPublisher;


        public UserDL(FundooContext context, IConfiguration configuration, TokenUtility tokenUtility,ILogger<UserDL> logger,RabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _configuration = configuration;
            _tokenUtility = tokenUtility;
            _logger = logger;
           _rabbitMQPublisher = rabbitMQPublisher;

        }


        public async Task<ResponseModel<User>> Registration(UserRegistration userRegistration)
        {
            _logger.LogInformation("Registration process started for {Email}", userRegistration.emailAddress);
            try
            {
                var user = _context.users.FirstOrDefault(user => user.EmailAddress.Equals(userRegistration.emailAddress));
                if (user != null)
                {
                    _logger.LogWarning("Attempt to register an already existing user: {Email}", userRegistration.emailAddress);
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

                // Publish message to RabbitMQ
                _rabbitMQPublisher.PublishToQueue("user_registration_queue", new { Email = user.EmailAddress, Name = user.Name });

                _logger.LogInformation("User {Email} registered successfully and message sent to RabbitMQ", userRegistration.emailAddress);

                _logger.LogInformation("User {Email} registered successfully", userRegistration.emailAddress);

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
                _logger.LogError(ex, "Registration failed: User already exists");
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
                _logger.LogError(ex, "An error occurred during registration for {Email}", userRegistration.emailAddress);
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

        
        public async Task<ResponseModel<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.users.ToListAsync();

                return new ResponseModel<List<User>>
                {
                    Data = users,
                    Message = "Users retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<User>>
                {
                    Data = null,
                    Message = "Error retrieving users",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        
        public async Task<ResponseModel<User>> GetUserById(int userId)
        {
            try
            {
                var user = await _context.users.FindAsync(userId);

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

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "Error retrieving user",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        
        public async Task<ResponseModel<bool>> DeleteUserById(int userId)
        {
            try
            {
                var user = await _context.users.FindAsync(userId);

                if (user == null)
                {
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                _context.users.Remove(user);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Data = true,
                    Message = "User deleted successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Message = "Error deleting user",
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

        public async Task<string> Login(Login user)
        {
            _logger.LogInformation("Login attempt for {Email}", user.emailAddress);
            var user_ = await ValidateUser(user); // Await the async UserLogin method

            if (user_ != null)
            {
                try
                {
                    var token = _tokenUtility.GenerateToken(user);
                    _logger.LogInformation("Login successful for {Email}", user.emailAddress);
                    return token;
                }
                catch (Exception ex)
                {
                    // Log exception (consider using a logging framework)
                    // Console.WriteLine($"Token generation failed: {ex.Message}");
                    _logger.LogError(ex, "Token generation failed for {Email}", user.emailAddress);
                }
            }
            _logger.LogWarning("Invalid login attempt for {Email}", user.emailAddress);
            return null;
        }

        public async Task<ResponseModel<string>> ForgotPassword(string email)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.EmailAddress.Equals(email));
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Success = false
                };
            }

            var newuser = new Login
            {
                emailAddress = email,
            };

            // Generate password reset token using TokenUtility
            var token = _tokenUtility.GenerateToken(newuser);

            // Send token via email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vaibhav", _configuration["SMTP:FromEmail"]));
            message.To.Add(new MailboxAddress(user.Name, user.EmailAddress));
            message.Subject = "Your Password Reset Token";
            message.Body = new TextPart("plain")
            {
                Text = $"Here is your password reset token:\n\n{token}\n\nUse this token to reset your password. The token is valid for 1 hour."
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return new ResponseModel<string>
            {
                Data = token,
                Message = "Password reset token sent successfully",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true
            };
        }


        public async Task<ResponseModel<string>> ResetPassword(string token, string newPassword)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                // Validate the token
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero  // Reduce default clock skew for more accuracy
                }, out SecurityToken validatedToken);

                // Extract email from the token's claims
                var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == "EmailAddress");
                if (emailClaim == null)
                {
                    return new ResponseModel<string>
                    {
                        Data = null,
                        Message = "Invalid token",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                
                var user = await _context.users.FirstOrDefaultAsync(u => u.EmailAddress.Equals(emailClaim.Value));
                if (user == null)
                {
                    return new ResponseModel<string>
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                // Hash the new password and update the user record
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "Password reset successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (SecurityTokenException)
            {
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "Invalid or expired token",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "An error occurred while resetting the password",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }



    }


}
