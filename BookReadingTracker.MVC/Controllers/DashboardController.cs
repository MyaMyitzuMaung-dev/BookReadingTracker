using BookReadingTracker.MVC.Models.Dashboard;
using BookReadingTracker.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApiService _api;

    public DashboardController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var dashboard = await _api.GetAsync<UserDashboardViewModel>("/api/dashboard/user");
            return View(dashboard ?? new UserDashboardViewModel());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new UserDashboardViewModel());
        }
    }
}
