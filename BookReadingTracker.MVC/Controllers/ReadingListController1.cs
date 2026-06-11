using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers
{
    public class ReadingListController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
