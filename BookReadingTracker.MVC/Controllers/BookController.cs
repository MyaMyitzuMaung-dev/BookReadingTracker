using BookReadingTracker.Domain.Features.Books;
using BookReadingTracker.Domain.Features.ReadingLists;
using BookReadingTracker.MVC.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookReadingTracker.MVC.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly BookService _bookService;
    private readonly ReadingListService _readingListService;

    public BookController(BookService bookService, ReadingListService readingListService)
    {
        _bookService = bookService;
        _readingListService = readingListService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, string? category)
    {
        try
        {
            var request = new GetBooksRequest { Search = search, Category = category };
            var response = await _bookService.GetBooksAsync(request);

            var vm = new BookListViewModel
            {
                Books = response.Books.Select(b => new BookItemViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    TotalPages = b.TotalPages,
                    Description = b.Description
                }).ToList(),
                Search = search,
                Category = category
            };

            return View(vm);
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
            var book = await _bookService.GetBookDetailAsync(id);

            var vm = new BookDetailViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Category = book.Category,
                TotalPages = book.TotalPages,
                Description = book.Description
            };

            return View(vm);
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
    public async Task<IActionResult> Create(AddBookRequest model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _bookService.AddBookAsync(model, userId);
            TempData["SuccessMessage"] = "Book created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
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
            var book = await _bookService.GetBookDetailAsync(id);
            var vm = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                Category = book.Category,
                TotalPages = book.TotalPages,
                Description = book.Description
            };
            return View(vm);
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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var request = new EditBookRequest
            {
                Title = model.Title,
                Author = model.Author,
                Category = model.Category,
                TotalPages = model.TotalPages,
                Description = model.Description
            };
            await _bookService.EditBookAsync(id, request, userId);
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
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
            await _bookService.DeleteBookAsync(id);
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
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _readingListService.AddToReadingListAsync(new AddToReadingListRequest { BookId = bookId }, userId);
            TempData["SuccessMessage"] = "Book added to your reading list.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Detail), new { id = bookId });
    }
}
