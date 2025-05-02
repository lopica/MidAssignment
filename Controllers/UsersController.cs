using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidAssignment.Services.Interfaces;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class UsersController(IApplicationUserServices userServices) : Controller
    {
        private readonly IApplicationUserServices _userServices = userServices;
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int currentPage = 1, [FromQuery] int limit = 5)
        {
            var result = await _userServices.GetUsers(currentPage, limit);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
