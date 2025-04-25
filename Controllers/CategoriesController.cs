using Microsoft.AspNetCore.Mvc;

namespace MidAssignment.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
