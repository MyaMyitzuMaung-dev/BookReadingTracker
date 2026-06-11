using BookReadingTracker.MVC.Models.ReadingList;
using BookReadingTracker.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class ReadingListController : Controller
{
    private readonly ApiService _api;

    public ReadingListController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var list = await _api.GetAsync<ReadingListViewModel>("/api/reading-list");
            return View(list ?? new ReadingListViewModel());
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new ReadingListViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await _api.DeleteAsync<ReadingListItemViewModel>($"/api/reading-list/{id}");
            TempData["SuccessMessage"] = "Book removed from reading list.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
