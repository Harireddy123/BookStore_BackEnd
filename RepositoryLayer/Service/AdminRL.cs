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


        public ForgotPasswordModel ForgetPassword(string email)
        {
            try
            {
                var admin = GetAdminByEmail(email);
                if (admin == null)
                    throw new Exception("Admin does not exist for the requested email");

                return new ForgotPasswordModel
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Token = GenerateToken(admin.Email, admin.Id, admin.Role)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing forgot password request", ex);
            }
        }

        public Admin GetAdminByEmail(string email)
        {
            try
            {
                return _dbContext.Admins.FirstOrDefault(admin => admin.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving admin by email", ex);
            }
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var admin = GetAdminByEmail(email);
                if (admin == null)
                    throw new Exception("Admin not found");

                if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmPassword)
                    throw new Exception("Password mismatch");

                admin.Password = Helper.EncodePassword(resetPasswordModel.NewPassword);
                _dbContext.Admins.Update(admin);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting password", ex);
            }
        }
    }
}
