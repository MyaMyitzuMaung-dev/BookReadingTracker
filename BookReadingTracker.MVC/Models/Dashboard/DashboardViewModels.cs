namespace BookReadingTracker.MVC.Models.Dashboard;

public class UserDashboardViewModel
{
    public int TotalBooksAdded { get; set; }
    public int CurrentlyReading { get; set; }
    public int CompletedBooks { get; set; }
    public int NotStarted { get; set; }
    public decimal OverallProgressPercent { get; set; }
}

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalBooks { get; set; }
    public int TotalCompletedReadings { get; set; }
}
