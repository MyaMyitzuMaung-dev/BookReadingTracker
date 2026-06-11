using BookReadingTracker.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace BookReadingTracker.Domain.Features.Books;

public class BookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetBooksResponse> GetBooksAsync(GetBooksRequest request)
    {
        var query = _db.Books.Where(b => !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(b => b.Title.Contains(request.Search));

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(b => b.Category == request.Category);

        var books = await query
            .OrderByDescending(b => b.CreatedDate)
            .Select(b => new BookItem
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                Category = b.Category,
                TotalPages = b.TotalPages,
                Description = b.Description
            })
            .ToListAsync();

        return new GetBooksResponse { Books = books };
    }

    public async Task<GetBookDetailResponse> GetBookDetailAsync(Guid bookId)
    {
        var book = await _db.Books
            .FirstOrDefaultAsync(b => b.BookId == bookId && !b.IsDeleted);

        if (book is null)
            throw new Exception("Book not found.");

        return new GetBookDetailResponse
        {
            BookId = book.BookId,
            Title = book.Title,
            Author = book.Author,
            Category = book.Category,
            TotalPages = book.TotalPages,
            Description = book.Description
        };
    }

    public async Task<AddBookResponse> AddBookAsync(AddBookRequest request, Guid createdBy)
    {
        var book = new Book
        {
            BookId = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            Category = request.Category,
            TotalPages = request.TotalPages,
            Description = request.Description,
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now,
            IsDeleted = false
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        return new AddBookResponse
        {
            BookId = book.BookId,
            Message = "Book added successfully."
        };
    }

    public async Task<EditBookResponse> EditBookAsync(Guid bookId, EditBookRequest request, Guid modifiedBy)
    {
        var book = await _db.Books
            .FirstOrDefaultAsync(b => b.BookId == bookId && !b.IsDeleted);

        if (book is null)
            throw new Exception("Book not found.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.Category = request.Category;
        book.TotalPages = request.TotalPages;
        book.Description = request.Description;
        book.ModifiedBy = modifiedBy;
        book.ModifiedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        return new EditBookResponse { Message = "Book updated successfully." };
    }

    public async Task<DeleteBookResponse> DeleteBookAsync(Guid bookId)
    {
        var book = await _db.Books
            .FirstOrDefaultAsync(b => b.BookId == bookId && !b.IsDeleted);

        if (book is null)
            throw new Exception("Book not found.");

        book.IsDeleted = true;
        book.ModifiedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        return new DeleteBookResponse { Message = "Book deleted successfully." };
    }
}
