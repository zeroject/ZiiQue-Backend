﻿using Application.DTOs;
using Domain;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public User GetUser(string username_, string password_);
        public User CreateNewUser(UserDTO userDTO_);
        public User UpdateUser(UserDTO userDTO_);
        public void DeleteUser(string email_);
    }
}