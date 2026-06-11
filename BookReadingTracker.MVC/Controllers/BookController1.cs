using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers
{
    public class BookController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
