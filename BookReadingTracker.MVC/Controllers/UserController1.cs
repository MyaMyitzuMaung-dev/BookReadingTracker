using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers
{
    public class UserController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
