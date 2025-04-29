using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok(new { test = "Ok" });
            //return Ok(new ApplicationResponse<List<string>>(false, 200, null, [""]));
        }
    }
}
