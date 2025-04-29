using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;
using MidAssignment.Services.Interfaces;

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
            ApplicationResponse result = await _authServices.RegisterAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model, [FromHeader(Name = "X-Public-Key")] string publicKey = "")
        {
            ApplicationResponse result = await _authServices.LoginAsync(model.Email, model.Password);
            if (!result.Success)
                return BadRequest(result);
            //if (string.IsNullOrWhiteSpace(publicKey))
            //{
            //    return BadRequest("Public key is missing.");
            //}
            var userResponse = (RegisterUserResponseDto)result.Content!;
            string userRole = userResponse.Role;
            string accessToken = _jWTServices.GenerateTokenWithPublicKey(model.Email, publicKey, false, userRole);
            Response.Cookies.Append("access_token", accessToken, _jWTServices.AccessTokenCookieOption());
            if (model.IsRememberLogin == true)
            {
                string refreshToken = _jWTServices.GenerateTokenWithPublicKey(model.Email, publicKey, true, userRole);
                Response.Cookies.Append("refresh_token", refreshToken, _jWTServices.RefreshTokenCookieOption());
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("refresh_token"))
            {
                Response.Cookies.Delete("refresh_token");
            }

            Response.Cookies.Delete("access_token");

            return Ok(new SuccessApplicationResponse<string>(StatusCodes.Status200OK, "Logged out successfully."));
        }


        //[HttpPost]
        //[Route("forgot-password")]
        //public IActionResult ForgotPassword()
        //{
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("reset-password")]
        //public IActionResult ResetPassword()
        //{
        //    return Ok();
        //}
    }
}
