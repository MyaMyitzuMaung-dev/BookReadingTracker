using BookReadingTracker.Database.AppDbContextModels;
using BookReadingTracker.Domain.Features.ReadingProgress;
using Microsoft.EntityFrameworkCore;

namespace BookReadingTracker.Domain.Features.ReadingProgress;

public class ReadingProgressService
{
    private readonly AppDbContext _db;

    public ReadingProgressService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetMyProgressResponse> GetMyProgressAsync(Guid userId, string? status)
    {
        var query = _db.ReadingProgresses
            .Where(rp => rp.UserId == userId && !rp.IsDeleted)
            .Join(_db.Books.Where(b => !b.IsDeleted),
                rp => rp.BookId,
                b => b.BookId,
                (rp, b) => new { rp, b });

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.rp.Status == status);

        var items = await query
            .OrderByDescending(x => x.rp.CreatedDate)
            .Select(x => new ProgressItem
            {
                ReadingProgressId = x.rp.ReadingProgressId,
                BookId = x.b.BookId,
                Title = x.b.Title,
                Author = x.b.Author,
                CurrentPage = x.rp.CurrentPage,
                TotalPages = x.b.TotalPages,
                ProgressPercent = x.b.TotalPages > 0
                    ? Math.Round((decimal)x.rp.CurrentPage / x.b.TotalPages * 100, 2)
                    : 0,
                Status = x.rp.Status,
                StartedDate = x.rp.StartedDate,
                CompletedDate = x.rp.CompletedDate
            })
            .ToListAsync();

        return new GetMyProgressResponse { Items = items };
    }

    public async Task<UpdateProgressResponse> UpdateProgressAsync(Guid progressId, UpdateProgressRequest request, Guid userId)
    {
        var progress = await _db.ReadingProgresses
            .Include(rp => rp.Book)
            .FirstOrDefaultAsync(rp => rp.ReadingProgressId == progressId && rp.UserId == userId && !rp.IsDeleted);

        if (progress is null)
            throw new Exception("Reading progress not found.");

        var totalPages = progress.Book.TotalPages;

        if (request.CurrentPage < 0 || request.CurrentPage > totalPages)
            throw new Exception($"Current page must be between 0 and {totalPages}.");

        progress.CurrentPage = request.CurrentPage;
        progress.ModifiedDate = DateTime.Now;
        progress.ModifiedBy = userId;

        // Auto status transition
        if (request.CurrentPage >= totalPages)
        {
            progress.Status = "Completed";
            progress.CompletedDate ??= DateTime.Now;
        }
        else if (request.CurrentPage > 0)
        {
            progress.Status = "Reading";
            progress.StartedDate ??= DateTime.Now;
        }
        else
        {
            progress.Status = "Not Started";
        }

        await _db.SaveChangesAsync();

        var progressPercent = totalPages > 0
            ? Math.Round((decimal)progress.CurrentPage / totalPages * 100, 2)
            : 0;

        return new UpdateProgressResponse
        {
            CurrentPage = progress.CurrentPage,
            TotalPages = totalPages,
            ProgressPercent = progressPercent,
            Status = progress.Status,
            Message = "Reading progress updated."
        };
    }
}
