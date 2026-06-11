using BookReadingTracker.Domain.Features.ReadingLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class ReadingListController : Controller
{
    private readonly ReadingListService _readingListService;

    public ReadingListController(ReadingListService readingListService)
    {
        _readingListService = readingListService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = await _readingListService.GetMyReadingListAsync(userId);
            return View(list);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new GetMyReadingListResponse());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _readingListService.RemoveFromReadingListAsync(id, userId);
            TempData["SuccessMessage"] = "Book removed from reading list.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
