using System;
using System.Collections.Generic;

namespace BookReadingTracker.Database.AppDbContextModels;

public partial class ReadingProgress
{
    public Guid ReadingProgressId { get; set; }

    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    public int CurrentPage { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? StartedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
