using BookReadingTracker.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace BookReadingTracker.Domain.Features.Dashboard;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetUserDashboardResponse> GetUserDashboardAsync(Guid userId)
    {
        var progresses = await _db.ReadingProgresses
            .Where(rp => rp.UserId == userId && !rp.IsDeleted)
            .Include(rp => rp.Book)
            .ToListAsync();

        var totalBooksAdded = progresses.Count;
        var currentlyReading = progresses.Count(rp => rp.Status == "Reading");
        var completedBooks = progresses.Count(rp => rp.Status == "Completed");
        var notStarted = progresses.Count(rp => rp.Status == "NotStarted");

        decimal overallPercent = 0;
        if (totalBooksAdded > 0)
        {
            var totalPercent = progresses
                .Where(rp => rp.Book.TotalPages > 0)
                .Sum(rp => (decimal)rp.CurrentPage / rp.Book.TotalPages * 100);
            overallPercent = Math.Round(totalPercent / totalBooksAdded, 2);
        }

        return new GetUserDashboardResponse
        {
            TotalBooksAdded = totalBooksAdded,
            CurrentlyReading = currentlyReading,
            CompletedBooks = completedBooks,
            NotStarted = notStarted,
            OverallProgressPercent = overallPercent
        };
    }

    public async Task<GetAdminDashboardResponse> GetAdminDashboardAsync()
    {
        var totalUsers = await _db.Users.CountAsync(u => !u.IsDeleted);
        var totalBooks = await _db.Books.CountAsync(b => !b.IsDeleted);
        var totalCompleted = await _db.ReadingProgresses.CountAsync(rp => rp.Status == "Completed" && !rp.IsDeleted);

        return new GetAdminDashboardResponse
        {
            TotalUsers = totalUsers,
            TotalBooks = totalBooks,
            TotalCompletedReadings = totalCompleted
        };
    }
}
