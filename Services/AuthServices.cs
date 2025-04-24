using Microsoft.AspNetCore.Identity;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using System.Data;
using System.Net.WebSockets;

namespace MidAssignment.Services
{
    public class AuthServices(UserManager<ApplicationUser> userManager) : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApplicationResponse<object>> RegisterAsync(RegisterDto registerDto, string role = "User")
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new ApplicationResponse<object>(false, StatusCodes.Status400BadRequest, result.Errors.Select(e => e.Description).ToList());
            }
            await _userManager.AddToRoleAsync(user, role);
            return new ApplicationResponse<object>(true, StatusCodes.Status201Created, null, new
            {
                user.Email,
                role,
                user.AvatarUrl
            });
        }

        public async Task<ApplicationResponse<object>> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApplicationResponse<object>(false, StatusCodes.Status400BadRequest, null, "email is invalid");
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return new ApplicationResponse<object>(false, StatusCodes.Status400BadRequest, null, "password is invalid");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var x = roles.FirstOrDefault();
            return new ApplicationResponse<object>(true, StatusCodes.Status200OK, null, 
                new RegisterUserResponseDto(user.Email,roles.FirstOrDefault()));
        }

    }
}
