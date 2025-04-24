using Microsoft.AspNetCore.Identity;
using MidAssignment.DTOs;

namespace MidAssignment.Services
{
    public interface IAuthServices
    {
        public Task<ApplicationResponse<object>> RegisterAsync(RegisterDto registerDto, string role = "User");
        public Task<ApplicationResponse<object>> LoginAsync(string email, string password);
    }
}
