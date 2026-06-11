namespace BookReadingTracker.Domain.Features.Books
{ 
// --- Get List ---
public class GetBooksRequest
{
    public string? Search { get; set; }
    public string? Category { get; set; }
}

public class GetBooksResponse
{
    public List<BookItem> Books { get; set; } = new();
}

public class BookItem
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}

// --- Get Detail ---
public class GetBookDetailResponse
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}

// --- Add ---
public class AddBookRequest
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}

public class AddBookResponse
{
    public Guid BookId { get; set; }
    public string Message { get; set; } = null!;
}

// --- Edit ---
public class EditBookRequest
{
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string? Category { get; set; }
    public int TotalPages { get; set; }
    public string? Description { get; set; }
}

public class EditBookResponse
{
    public string Message { get; set; } = null!;
}

// --- Delete ---
public class DeleteBookResponse
{
    public string Message { get; set; } = null!;
}
}
