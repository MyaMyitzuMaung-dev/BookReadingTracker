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
        var notStarted = progresses.Count(rp => rp.Status == "Not Started");

        decimal overallPercent = 0;
        if (totalBooksAdded > 0)
        {
            var totalPercent = progresses
                .Where(rp => rp.Book.TotalPages > 0)
                .Sum(rp => (decimal)rp.CurrentPage / rp.Book.TotalPages * 100);
            overallPercent = Math.Round(totalPercent / totalBooksAdded, 2);
        }

        var totalPagesRead = progresses
            .Sum(rp => rp.Status == "Completed" ? rp.Book.TotalPages : rp.CurrentPage);

        var achievementFirstBook = completedBooks >= 1;
        var achievementFiveBooks = completedBooks >= 5;
        var achievementSevenDays = HasSevenConsecutiveDays(progresses);
        var achievementThousandPages = totalPagesRead >= 1000;

        var badge = completedBooks switch
        {
            >= 10 => "🥇 Book Master",
            >= 5  => "🥈 Active Reader",
            >= 1  => "🥉 Beginner Reader",
            _     => ""
        };

        return new GetUserDashboardResponse
        {
            TotalBooksAdded = totalBooksAdded,
            CurrentlyReading = currentlyReading,
            CompletedBooks = completedBooks,
            NotStarted = notStarted,
            OverallProgressPercent = overallPercent,
            TotalPagesRead = totalPagesRead,
            AchievementFirstBook = achievementFirstBook,
            AchievementFiveBooks = achievementFiveBooks,
            AchievementSevenDays = achievementSevenDays,
            AchievementThousandPages = achievementThousandPages,
            Badge = badge
        };
    }

    private static bool HasSevenConsecutiveDays(List<Database.AppDbContextModels.ReadingProgress> progresses)
    {
        var distinctDates = progresses
            .Select(rp => rp.CreatedDate.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        if (distinctDates.Count < 7)
            return false;

        var consecutive = 1;
        for (var i = 1; i < distinctDates.Count; i++)
        {
            if ((distinctDates[i] - distinctDates[i - 1]).Days == 1)
            {
                consecutive++;
                if (consecutive >= 7)
                    return true;
            }
            else
            {
                consecutive = 1;
            }
        }

        return false;
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
