namespace BookReadingTracker.MVC.Models.Book;

public class BookItemViewModel
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}

public class BookListViewModel
{
    public List<BookItemViewModel> Books { get; set; } = new();
    public string? Search { get; set; }
    public string? Category { get; set; }
}

public class BookDetailViewModel
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
    public bool IsInReadingList { get; set; }
}

public class EditBookViewModel
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}
