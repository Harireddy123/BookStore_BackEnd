using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BussinessLayer.Interface
{
    public interface IAdminBL
    {
        public Admin RegisterAdmin(AdminRegisterModel model);
        public string Login(AdminLoginModel model);
        public bool EmailExists(string email);
        ForgotPasswordModel ForgetPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel model);
    }
}
