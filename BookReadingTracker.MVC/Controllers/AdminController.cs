using BookReadingTracker.MVC.Models.Dashboard;
using BookReadingTracker.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApiService _api;

    public AdminController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var data = await _api.GetAsync<AdminDashboardViewModel>("/api/dashboard/admin");
            return View(data ?? new AdminDashboardViewModel());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new AdminDashboardViewModel());
        }
    }
}
