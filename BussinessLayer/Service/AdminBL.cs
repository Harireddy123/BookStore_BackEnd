using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class AdminBL : IAdminBL
    {
        private readonly IAdminRL _adminRepository;
        public AdminBL(IAdminRL adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public Admin RegisterAdmin(AdminRegisterModel model) => _adminRepository.RegisterAdmin(model);

        public string Login(AdminLoginModel model) => _adminRepository.Login(model);

        public bool EmailExists(string email) => _adminRepository.EmailExists(email);
    }
}
