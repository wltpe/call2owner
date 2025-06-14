using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class CabCompany
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Logo { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<ResidentFrequentlyEntry> ResidentFrequentlyEntry { get; set; } = new List<ResidentFrequentlyEntry>();
}
