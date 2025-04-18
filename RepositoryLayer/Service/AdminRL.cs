using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helpers;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AdminRL : IAdminRL
    {
        private readonly BookContext _dbContext;
        private readonly IConfiguration _configuration;

        public AdminRL(BookContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public bool EmailExists(string email) =>

            _dbContext.Admins.Any(e => e.Email == email);

        public string Login(AdminLoginModel model)
        {
            try
            {
                var admin = _dbContext.Admins.FirstOrDefault(u => u.Email == model.Email);
                if (admin == null)
                    return null;

                string decodedPassword = Helper.Decode(admin.Password);
                return decodedPassword == model.Password ? GenerateToken(admin.Email, admin.Id, admin.Role) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login", ex);
            }
        }

        public Admin RegisterAdmin(AdminRegisterModel model)
        {
            try
            {
                Admin admin = new Admin
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = Helper.EncodePassword(model.Password),
                    Role = model.Role
                };

                _dbContext.Add(admin);
                _dbContext.SaveChanges();

                return admin;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering admin", ex);
            }
        }

        private string GenerateToken(string Email, int adminId, string Role)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim("custom_email", Email),
                    new Claim("id", adminId.ToString()),
                    new Claim("custom_role", Role),
                };
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.Now.AddMinutes(50),
                    signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating JWT token", ex);
            }
        }
    }
}
