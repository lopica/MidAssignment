using Microsoft.AspNetCore.Mvc;
using MidAssignment.DTOs;

namespace MidAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status201Created)]
        //[ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ErrorDTO), StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNewBook()
        {
            return View();
        }
    }
}
