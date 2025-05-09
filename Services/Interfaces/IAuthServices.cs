﻿using MidAssignment.DTOs;
using MidAssignment.DTOs.Auth;

namespace MidAssignment.Services.Interfaces
{
    public interface IAuthServices
    {
        public Task<ApplicationResponse> RegisterAsync(RegisterDto registerDto, string role = "User");
        public Task<ApplicationResponse> LoginAsync(string email, string password);
    }
}
