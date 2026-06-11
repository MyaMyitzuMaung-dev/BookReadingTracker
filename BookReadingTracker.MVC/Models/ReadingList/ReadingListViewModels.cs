namespace BookReadingTracker.MVC.Models.ReadingList;

public class ReadingListItemViewModel
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

public class ReadingListViewModel
{
    public List<ReadingListItemViewModel> Items { get; set; } = new();
}
