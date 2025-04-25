using Microsoft.AspNetCore.Mvc;

namespace MidAssignment.Controllers
{
    public class BorrowingRequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
