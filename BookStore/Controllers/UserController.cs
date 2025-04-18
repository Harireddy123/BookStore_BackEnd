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

        public UserController(IUserBL userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            try
            {

                if (model == null)
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid registration details provided" });

                if (_userManager.EmailExists(model.Email))
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email already exists" });

                var result = _userManager.Registeration(model);

                if (result != null)
                {
                    return Ok(new ResponseModel<User>
                    { Success = true, Message = "Registered successfully", Data = result });
                }

                return BadRequest(new ResponseModel<User>
                { Success = false, Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            try
            {

                if (model == null)
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid login details" });

                var result = _userManager.Login(model);

                if (result != null)
                {
                    return Ok(new ResponseModel<string>
                    { Success = true, Message = "Login successful", Data = result });
                }

                return Unauthorized(new ResponseModel<string>
                { Success = false, Message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpGet("forgotPassword")]
        public IActionResult ForgetPassword(string email)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email is required" });

                ForgotPasswordModel forgotPasswordModel = _userManager.ForgetPassword(email);

                if (forgotPasswordModel == null)
                {
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "User with this email does not exist" });
                }

                // Send email
                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);

                return Ok(new { Success = true, Message = "Password reset email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An error occurred while processing your request", Data = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(ResetPasswordModel request)
        {
            try
            {
                string email = User.Claims.FirstOrDefault(c => c.Type == "custom_email")?.Value;

                if (email == null)
                {
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }

                var result = _userManager.ResetPassword(email, request);

                if (result)
                {
                    return Ok(new { success = true, message = "Password reset successful" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Password reset unsuccessful" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while resetting the password. Please try again later.", Data = ex.Message });
            }
        }
    }
}
