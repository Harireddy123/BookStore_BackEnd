using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BussinessLayer.Interface
{
    public interface IUserBL
    {
        public User Registeration(RegisterModel model);

        public string Login(LoginModel model);

        public bool EmailExists(string email);
    }
}
