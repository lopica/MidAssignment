using Microsoft.AspNetCore.Identity;
using MidAssignment.Domain;
using MidAssignment.DTOs;
using MidAssignment.Services.Interfaces;
using System.Data;

namespace MidAssignment.Services
{
    public class AuthServices(UserManager<ApplicationUser> userManager) : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApplicationResponse> RegisterAsync(RegisterDto registerDto, string role = "User")
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
            };
            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, result.Errors.Select(e => e.Description).ToList());
            }
            await _userManager.AddToRoleAsync(user, role);
            return new SuccessApplicationResponse<object>(StatusCodes.Status201Created, content: new
            {
                user.Email,
                role,
                user.AvatarUrl
            });
        }

        public async Task<ApplicationResponse> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["email is invalid"]);
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return new ErrorApplicationResponse(StatusCodes.Status400BadRequest, ["password is invalid"]);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var x = roles.FirstOrDefault();
            return new SuccessApplicationResponse<object>(StatusCodes.Status200OK, 
                content: new RegisterUserResponseDto(user.Email!,roles.FirstOrDefault()!));
        }

    }
}
