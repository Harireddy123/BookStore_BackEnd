using BussinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Linq;

namespace BookStore.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBL userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            _logger.LogInformation("Register endpoint called.");

            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Registration failed: model is null.");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid registration details provided" });
                }

                _logger.LogInformation("Checking if email already exists: {Email}", model.Email);

                if (_userManager.EmailExists(model.Email))
                {
                    _logger.LogWarning("Registration failed: Email {Email} already exists.", model.Email);
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email already exists" });
                }

                var result = _userManager.Registeration(model);

                if (result != null)
                {
                    _logger.LogInformation("User registered successfully: {Email}", model.Email);
                    return Ok(new ResponseModel<User>
                    { Success = true, Message = "Registered successfully", Data = result });
                }

                _logger.LogWarning("Registration failed for email: {Email}", model.Email);
                return BadRequest(new ResponseModel<User>
                { Success = false, Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during registration.");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            _logger.LogInformation("Login endpoint called.");

            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Login failed: model is null.");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid login details" });
                }

                var result = _userManager.Login(model);

                if (result != null)
                {
                    _logger.LogInformation("Login successful for user: {Email}", model.Email);
                    return Ok(new ResponseModel<LoginResponseModel>
                    { Success = true, Message = "Login successful", Data = result });
                }

                _logger.LogWarning("Login failed: Invalid email or password for {Email}", model.Email);
                return Unauthorized(new ResponseModel<string>
                { Success = false, Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login.");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpGet("forgotPassword")]
        public IActionResult ForgetPassword(string email)
        {
            _logger.LogInformation("ForgetPassword endpoint called with email: {Email}", email);

            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Forgot password failed: Email is empty.");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email is required" });
                }

                ForgotPasswordModel forgotPasswordModel = _userManager.ForgetPassword(email);

                if (forgotPasswordModel == null)
                {
                    _logger.LogWarning("Forgot password failed: No user found with email: {Email}", email);
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "User with this email does not exist" });
                }

                _logger.LogInformation("Sending password reset email to: {Email}", forgotPasswordModel.Email);

                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

                return Ok(new { Success = true, Message = "Password reset email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during forget password process.");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An error occurred while processing your request", Data = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel request)
        {
            _logger.LogInformation("ResetPassword endpoint called.");

            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == "custom_email")?.Value;

                if (email == null)
                {
                    _logger.LogWarning("Reset password failed: email claim missing.");
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }

                var result = _userManager.ResetPassword(email, request);

                if (result)
                {
                    _logger.LogInformation("Password reset successful for: {Email}", email);
                    return Ok(new { success = true, message = "Password reset successful" });
                }
                else
                {
                    _logger.LogWarning("Password reset unsuccessful for: {Email}", email);
                    return BadRequest(new { success = false, message = "Password reset unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during password reset.");
                return StatusCode(500, new { success = false, message = "An error occurred while resetting the password. Please try again later.", Data = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromBody] RefreshTokenModel model)
        {
            _logger.LogInformation("RefreshToken endpoint called.");

            var result = _userManager.RefreshToken(model.RefreshToken);

            if (result == null)
            {
                _logger.LogWarning("Refresh token failed: Invalid or expired token.");
                return Unauthorized(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid or expired refresh token"
                });
            }

            _logger.LogInformation("Token refreshed successfully.");
            return Ok(new ResponseModel<LoginResponseModel>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = result
            });
        }
    }
}
