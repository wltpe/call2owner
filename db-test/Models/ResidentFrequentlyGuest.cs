using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class ResidentFrequentlyGuest
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public string Type { get; set; } = null!;

    public bool? IsPrivateEntry { get; set; }

    public DateOnly? SelectDate { get; set; }

    public TimeOnly? StartingFrom { get; set; }

    public string? ValidFor { get; set; }

    public string? AllowEntryForNext { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Guests { get; set; } = null!;

    public string? Note { get; set; }

    public string UniqueEntryNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual Resident Resident { get; set; } = null!;
}
