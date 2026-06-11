namespace BookReadingTracker.MVC.Models.ReadingProgress;

public class ProgressItemViewModel
{
    public Guid ReadingProgressId { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public decimal ProgressPercent { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? StartedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

public class ProgressListViewModel
{
    public List<ProgressItemViewModel> Items { get; set; } = new();
    public string? StatusFilter { get; set; }
}


