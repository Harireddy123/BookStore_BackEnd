using BussinessLayer.Interface;
using System;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace BookStore.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminBL _adminManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminBL adminManager, ILogger<AdminController> logger)
        {
            _adminManager = adminManager;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Register(AdminRegisterModel model)
        {
            _logger.LogInformation("Admin registration attempt");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Registration failed: model is null");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid registration details provided" });
                }

                if (_adminManager.EmailExists(model.Email))
                {
                    _logger.LogWarning("Registration failed: email already exists - {Email}", model.Email);
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email already exists" });
                }

                var result = _adminManager.RegisterAdmin(model);

                if (result != null)
                {
                    _logger.LogInformation("Admin registered successfully: {Email}", model.Email);
                    return Ok(new ResponseModel<Admin>
                    { Success = true, Message = "Registered successfully", Data = result });
                }

                _logger.LogWarning("Admin registration failed for email: {Email}", model.Email);
                return BadRequest(new ResponseModel<Admin>
                { Success = false, Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during admin registration");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(AdminLoginModel model)
        {
            _logger.LogInformation("Admin login attempt");
            try
            {
                if (model == null)
                {
                    _logger.LogWarning("Login failed: model is null");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid login details" });
                }

                var result = _adminManager.Login(model);

                if (result != null)
                {
                    _logger.LogInformation("Admin logged in successfully: {Email}", model.Email);
                    return Ok(new ResponseModel<string>
                    { Success = true, Message = "Login successful", Data = result });
                }

                _logger.LogWarning("Login failed: Invalid email or password - {Email}", model.Email);
                return Unauthorized(new ResponseModel<string>
                { Success = false, Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during admin login");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpGet("forgotpassword")]
        public IActionResult ForgetPassword(string email)
        {
            _logger.LogInformation("Admin forgot password attempt for email: {Email}", email);
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Forget password failed: email is empty");
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email is required" });
                }

                ForgotPasswordModel forgotPasswordModel = _adminManager.ForgetPassword(email);

                if (forgotPasswordModel == null)
                {
                    _logger.LogWarning("Forget password failed: Admin not found - {Email}", email);
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Admin with this email does not exist" });
                }

                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);
                _logger.LogInformation("Password reset email sent to {Email}", email);

                return Ok(new { Success = true, Message = "Password reset email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during forget password process");
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An error occurred while processing your request", Data = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel request)
        {
            _logger.LogInformation("Admin password reset attempt");
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == "custom_email")?.Value;

                if (email == null)
                {
                    _logger.LogWarning("Reset password failed: Token missing or invalid");
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }

                var result = _adminManager.ResetPassword(email, request);

                if (result)
                {
                    _logger.LogInformation("Password reset successful for {Email}", email);
                    return Ok(new { success = true, message = "Password reset successful" });
                }
                else
                {
                    _logger.LogWarning("Password reset unsuccessful for {Email}", email);
                    return BadRequest(new { success = false, message = "Password reset unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during password reset");
                return StatusCode(500, new { success = false, message = "An error occurred while resetting the password. Please try again later.", Data = ex.Message });
            }
        }
    }
}
