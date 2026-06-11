using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers
{
    public class DashboardController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
