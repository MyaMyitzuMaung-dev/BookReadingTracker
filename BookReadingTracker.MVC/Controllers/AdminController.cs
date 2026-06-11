using BookReadingTracker.Domain.Features.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly DashboardService _dashboardService;

    public AdminController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var data = await _dashboardService.GetAdminDashboardAsync();
            return View(data);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new GetAdminDashboardResponse());
        }
    }
}
