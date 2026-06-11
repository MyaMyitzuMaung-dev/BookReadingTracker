namespace BookReadingTracker.Domain.Features.ReadingLists;

// --- Get My Reading List ---
public class GetMyReadingListResponse
{
    public List<ReadingListItem> Items { get; set; } = new();
}

public class ReadingListItem
{
    public Guid ReadingListId { get; set; }
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int TotalPages { get; set; }
    public DateTime AddedDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal ProgressPercent { get; set; }
}

// --- Add To Reading List ---
public class AddToReadingListRequest
{
    public Guid BookId { get; set; }
}

public class AddToReadingListResponse
{
    public Guid ReadingListId { get; set; }
    public string Message { get; set; } = null!;
}

// --- Remove From Reading List ---
public class RemoveFromReadingListResponse
{
    public string Message { get; set; } = null!;
}
