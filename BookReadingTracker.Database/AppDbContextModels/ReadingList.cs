using System;
using System.Collections.Generic;

namespace BookReadingTracker.Database.AppDbContextModels;

public partial class ReadingList
{
    public Guid ReadingListId { get; set; }

    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    public DateTime AddedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
