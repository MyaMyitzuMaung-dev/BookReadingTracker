namespace BookReadingTracker.Domain.Features.ReadingProgress;

// --- Get My Progress ---
public class GetMyProgressResponse
{
    public List<ProgressItem> Items { get; set; } = new();
}

public class ProgressItem
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

// --- Update Progress ---
public class UpdateProgressRequest
{
    public int CurrentPage { get; set; }
}

public class UpdateProgressResponse
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public decimal ProgressPercent { get; set; }
    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}
