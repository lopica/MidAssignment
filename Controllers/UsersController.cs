using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        }
    }
}
