using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        public User RegisterUser(RegisterModel model);
        public string Login(LoginModel model);
        public bool EmailExists(string email);
        public ForgotPasswordModel ForgetPassword(string email);
        public bool ResetPassword(string email, ResetPasswordModel resetPasswordModel);
    }
}
