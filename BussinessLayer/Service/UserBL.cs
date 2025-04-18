using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRepository;
        public UserBL(IUserRL userRepository)
        {
            _userRepository = userRepository;
        }

        public User Registeration(RegisterModel model) => _userRepository.RegisterUser(model);

        public string Login(LoginModel model) => _userRepository.Login(model);

        public bool EmailExists(string email) => _userRepository.EmailExists(email);

        public ForgotPasswordModel ForgetPassword(string email) => _userRepository.ForgetPassword(email);

        public bool ResetPassword(string email, ResetPasswordModel model) => _userRepository.ResetPassword(email, model);

    }
}
