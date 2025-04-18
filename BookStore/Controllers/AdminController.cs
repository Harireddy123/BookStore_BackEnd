using BussinessLayer.Interface;
using System;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminBL _adminManager;

        public AdminController(IAdminBL adminManager)
        {
            _adminManager = adminManager;
        }

        [HttpPost("Register")]
        public IActionResult Register(AdminRegisterModel model)
        {
            try
            {

                if (model == null)
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid registration details provided" });

                if (_adminManager.EmailExists(model.Email))
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Email already exists" });

                var result = _adminManager.RegisterAdmin(model);

                if (result != null)
                {
                    return Ok(new ResponseModel<Admin>
                    { Success = true, Message = "Registered successfully", Data = result });
                }

                return BadRequest(new ResponseModel<Admin>
                { Success = false, Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                { Success = false, Message = "An internal error occurred", Data = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(AdminLoginModel model)
        {
            try
            {

                if (model == null)
                    return BadRequest(new ResponseModel<string>
                    { Success = false, Message = "Invalid login details" });

                var result = _adminManager.Login(model);

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
