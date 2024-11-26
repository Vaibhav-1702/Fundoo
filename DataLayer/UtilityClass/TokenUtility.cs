using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.UtilityClass
{
    public class TokenUtility
    {
        private readonly IConfiguration _configuration;

        public TokenUtility(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Login user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("EmailAddress", user.emailAddress)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
