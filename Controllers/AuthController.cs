using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;
using MidAssignment.Services;
using System.Net;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthServices authServices, IJWTServices jWTServices) : Controller
    {
        private readonly IAuthServices _authServices = authServices;
        private readonly IJWTServices _jWTServices = jWTServices;

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            ApplicationResponse<object> result = await _authServices.RegisterAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model, [FromHeader(Name = "X-Public-Key")] string publicKey = "")
        {
            ApplicationResponse<object> result = await _authServices.LoginAsync(model.Email, model.Password);
            if (!result.Success)
                return BadRequest(result);
            //if (string.IsNullOrWhiteSpace(publicKey))
            //{
            //    return BadRequest("Public key is missing.");
            //}
            var userResponse = (RegisterUserResponseDto)result.Content!;
            string userRole = userResponse.Role;
            string accessToken = _jWTServices.GenerateTokenWithPublicKey(model.Email, publicKey, false, userRole);
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, 
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });
            if (model.IsRememberLogin == true)
            {
                string refreshToken = _jWTServices.GenerateTokenWithPublicKey(model.Email, publicKey, true, userRole);
                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMonths(1)
                });
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.ContainsKey("refresh_token"))
            {
                Response.Cookies.Delete("refresh_token");
            }

            Response.Cookies.Delete("access_token");

            return Ok(new ApplicationResponse<object>(true, StatusCodes.Status200OK, null, "Logged out successfully."));
        }


        [HttpPost]
        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return Ok();
        }

        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword()
        {
            return Ok();
        }
    }
}
