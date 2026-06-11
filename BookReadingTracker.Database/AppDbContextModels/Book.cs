using System;
using System.Collections.Generic;

namespace BookReadingTracker.Database.AppDbContextModels;

public partial class Book
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Category { get; set; }

    public int TotalPages { get; set; }

    public string? Description { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ReadingList> ReadingLists { get; set; } = new List<ReadingList>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();
}
