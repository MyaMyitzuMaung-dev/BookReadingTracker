namespace BookReadingTracker.Domain.Features.Dashboard;

// --- User Dashboard ---
public class GetUserDashboardResponse
{
    public int TotalBooksAdded { get; set; }
    public int CurrentlyReading { get; set; }
    public int CompletedBooks { get; set; }
    public int NotStarted { get; set; }
    public decimal OverallProgressPercent { get; set; }

    public int TotalPagesRead { get; set; }
    public bool AchievementFirstBook { get; set; }
    public bool AchievementFiveBooks { get; set; }
    public bool AchievementSevenDays { get; set; }
    public bool AchievementThousandPages { get; set; }
    public string Badge { get; set; } = "";
}

// --- Admin Dashboard ---
public class GetAdminDashboardResponse
{
    public int TotalUsers { get; set; }
    public int TotalBooks { get; set; }
    public int TotalCompletedReadings { get; set; }
}
