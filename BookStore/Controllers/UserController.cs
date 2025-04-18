using BussinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using System;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userManager;

        public UserController(IUserBL userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("Registration")]
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
    }
}
