using BookReadingTracker.MVC.Models.Book;
using BookReadingTracker.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly ApiService _api;

    public BookController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? category)
    {
        try
        {
            var query = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrWhiteSpace(category)) query.Add($"category={Uri.EscapeDataString(category)}");

            var url = "/api/books" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
            var result = await _api.GetAsync<BookListViewModel>(url);

            result ??= new BookListViewModel();
            result.Search = search;
            result.Category = category;

            return View(result);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(new BookListViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        try
        {
            var book = await _api.GetAsync<BookDetailViewModel>($"/api/books/{id}");
            if (book is null) return NotFound();
            return View(book);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateBookViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _api.PostAsync<CreateBookViewModel, BookDetailViewModel>("/api/books", model);
            TempData["SuccessMessage"] = "Book created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var book = await _api.GetAsync<EditBookViewModel>($"/api/books/{id}");
            if (book is null) return NotFound();
            return View(book);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, EditBookViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _api.PutAsync<EditBookViewModel, BookDetailViewModel>($"/api/books/{id}", model);
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _api.DeleteAsync<BookDetailViewModel>($"/api/books/{id}");
            TempData["SuccessMessage"] = "Book deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddToReadingList(Guid bookId)
    {
        try
        {
            await _api.PostAsync<object, object>("/api/reading-list", new { bookId });
            TempData["SuccessMessage"] = "Book added to your reading list.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Detail), new { id = bookId });
    }
}
