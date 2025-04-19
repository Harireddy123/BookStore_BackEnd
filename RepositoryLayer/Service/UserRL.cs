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
    public class UserRL : IUserRL
    {
        private readonly BookContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserRL(BookContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }

        public bool EmailExists(string email) =>

            _dbContext.Users.Any(e => e.Email == email);

        public User RegisterUser(RegisterModel model)
        {
            try
            {
                User user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = Helper.EncodePassword(model.Password),
                };

                _dbContext.Add(user);
                _dbContext.SaveChanges();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user", ex);
            }
        }

        public LoginResponseModel Login(LoginModel model)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null || Helper.Decode(user.Password) != model.Password)
                    return null;

                return new LoginResponseModel
                {
                    AccessToken = GenerateToken(user.Email, user.Id, user.Role),
                    RefreshToken = GenerateRefreshToken(user.Email, user.Id, user.Role)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login", ex);
            }
        }

        private string GenerateToken(string Email, int userId, string Role)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim("custom_email", Email),
                    new Claim("id", userId.ToString()),
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

        private string GenerateRefreshToken(string Email, int userId, string Role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim("custom_email", Email),
        new Claim("id", userId.ToString()),
        new Claim("custom_role", Role),
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // refresh token valid for 7 days
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public ForgotPasswordModel ForgetPassword(string email)
        {
            try
            {
                var user = GetUserByEmail(email);
                if (user == null)
                    throw new Exception("User does not exist for the requested email");

                return new ForgotPasswordModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Token = GenerateToken(user.Email, user.Id, user.Role)
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error processing forgot password request", ex);
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                return _dbContext.Users.FirstOrDefault(user => user.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by email", ex);
            }
        }

        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var user = GetUserByEmail(email);
                if (user == null)
                    throw new Exception("User not found");

                if (resetPasswordModel.NewPassword != resetPasswordModel.ConfirmPassword)
                    throw new Exception("Password mismatch");

                user.Password = Helper.EncodePassword(resetPasswordModel.NewPassword);
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error resetting password", ex);
            }
        }
        public LoginResponseModel RefreshToken(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshKey"]);

                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "custom_email").Value;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var role = jwtToken.Claims.First(x => x.Type == "custom_role").Value;

                return new LoginResponseModel
                {
                    AccessToken = GenerateToken(email, userId, role),
                    RefreshToken = GenerateRefreshToken(email, userId, role)
                };
            }
            catch
            {
                return null;
            }
        }

    }
}
