using BookReadingTracker.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using DbReadingProgress = BookReadingTracker.Database.AppDbContextModels.ReadingProgress;

namespace BookReadingTracker.Domain.Features.ReadingLists;

public class ReadingListService
{
    private readonly AppDbContext _db;

    public ReadingListService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetMyReadingListResponse> GetMyReadingListAsync(Guid userId)
    {
        var items = await _db.ReadingLists
            .Where(rl => rl.UserId == userId && !rl.IsDeleted)
            .Join(_db.Books.Where(b => !b.IsDeleted),
                rl => rl.BookId,
                b => b.BookId,
                (rl, b) => new { rl, b })
            .GroupJoin(
                _db.ReadingProgresses.Where(rp => rp.UserId == userId && !rp.IsDeleted),
                x => x.rl.BookId,
                rp => rp.BookId,
                (x, rps) => new { x.rl, x.b, rps })
            .SelectMany(
                x => x.rps.DefaultIfEmpty(),
                (x, rp) => new ReadingListItem
                {
                    ReadingListId = x.rl.ReadingListId,
                    BookId = x.b.BookId,
                    Title = x.b.Title,
                    Author = x.b.Author,
                    TotalPages = x.b.TotalPages,
                    AddedDate = x.rl.AddedDate,
                    Status = rp != null ? rp.Status : "NotStarted",
                    ProgressPercent = rp != null && x.b.TotalPages > 0
                        ? Math.Round((decimal)rp.CurrentPage / x.b.TotalPages * 100, 2)
                        : 0
                })
            .OrderByDescending(x => x.AddedDate)
            .ToListAsync();

        return new GetMyReadingListResponse { Items = items };
    }

    public async Task<AddToReadingListResponse> AddToReadingListAsync(AddToReadingListRequest request, Guid userId)
    {
        var book = await _db.Books
            .FirstOrDefaultAsync(b => b.BookId == request.BookId && !b.IsDeleted);

        if (book is null)
            throw new Exception("Book not found.");

        var alreadyAdded = await _db.ReadingLists
            .AnyAsync(rl => rl.UserId == userId && rl.BookId == request.BookId && !rl.IsDeleted);

        if (alreadyAdded)
            throw new Exception("Book is already in your reading list.");

        var readingListId = Guid.NewGuid();

        var readingList = new ReadingList
        {
            ReadingListId = readingListId,
            UserId = userId,
            BookId = request.BookId,
            AddedDate = DateTime.Now,
            CreatedBy = userId,
            CreatedDate = DateTime.Now,
            IsDeleted = false
        };

        var readingProgress = new DbReadingProgress
        {
            ReadingProgressId = Guid.NewGuid(),
            UserId = userId,
            BookId = request.BookId,
            CurrentPage = 0,
            Status = "NotStarted",
            CreatedBy = userId,
            CreatedDate = DateTime.Now,
            IsDeleted = false
        };

        _db.ReadingLists.Add(readingList);
        _db.ReadingProgresses.Add(readingProgress);
        await _db.SaveChangesAsync();

        return new AddToReadingListResponse
        {
            ReadingListId = readingListId,
            Message = "Book added to reading list."
        };
    }

    public async Task<RemoveFromReadingListResponse> RemoveFromReadingListAsync(Guid readingListId, Guid userId)
    {
        var readingList = await _db.ReadingLists
            .FirstOrDefaultAsync(rl => rl.ReadingListId == readingListId && rl.UserId == userId && !rl.IsDeleted);

        if (readingList is null)
            throw new Exception("Reading list entry not found.");

        readingList.IsDeleted = true;
        readingList.ModifiedDate = DateTime.Now;
        readingList.ModifiedBy = userId;

        await _db.SaveChangesAsync();

        return new RemoveFromReadingListResponse { Message = "Book removed from reading list." };
    }
}
