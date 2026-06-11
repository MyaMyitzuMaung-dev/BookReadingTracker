using BookReadingTracker.MVC.Models.ReadingProgress;
using BookReadingTracker.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class ReadingProgressController : Controller
{
    private readonly ApiService _api;

    public ReadingProgressController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        try
        {
            var url = "/api/reading-progress" + (!string.IsNullOrWhiteSpace(status)
                ? $"?status={Uri.EscapeDataString(status)}" : "");

            var result = await _api.GetAsync<ProgressListViewModel>(url);
            result ??= new ProgressListViewModel();
            result.StatusFilter = status;
            return View(result);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new ProgressListViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(Guid id, UpdateProgressViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid page number.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _api.PutAsync<UpdateProgressViewModel, ProgressItemViewModel>(
                $"/api/reading-progress/{id}", model);
            TempData["SuccessMessage"] = "Progress updated.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
