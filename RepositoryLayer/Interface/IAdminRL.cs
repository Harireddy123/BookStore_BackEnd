using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAdminRL
    {
        public Admin RegisterAdmin(AdminRegisterModel model);
        public string Login(AdminLoginModel model);
        public bool EmailExists(string email);
    }
}
