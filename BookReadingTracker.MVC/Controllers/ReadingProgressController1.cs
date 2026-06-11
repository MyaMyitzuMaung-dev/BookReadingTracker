using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers
{
    public class ReadingProgressController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
