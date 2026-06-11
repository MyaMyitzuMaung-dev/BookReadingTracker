using BookReadingTracker.Domain.Features.ReadingProgress;
using BookReadingTracker.MVC.Models.ReadingProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class ReadingProgressController : Controller
{
    private readonly ReadingProgressService _readingProgressService;

    public ReadingProgressController(ReadingProgressService readingProgressService)
    {
        _readingProgressService = readingProgressService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var data = await _readingProgressService.GetMyProgressAsync(userId, status);

            var vm = new ProgressListViewModel
            {
                Items = data.Items.Select(i => new ProgressItemViewModel
                {
                    ReadingProgressId = i.ReadingProgressId,
                    BookId = i.BookId,
                    Title = i.Title,
                    Author = i.Author,
                    CurrentPage = i.CurrentPage,
                    TotalPages = i.TotalPages,
                    ProgressPercent = i.ProgressPercent,
                    Status = i.Status,
                    StartedDate = i.StartedDate,
                    CompletedDate = i.CompletedDate
                }).ToList(),
                StatusFilter = status
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new ProgressListViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(Guid id, UpdateProgressRequest model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid page number.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _readingProgressService.UpdateProgressAsync(id, model, userId);
            TempData["SuccessMessage"] = "Progress updated.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
