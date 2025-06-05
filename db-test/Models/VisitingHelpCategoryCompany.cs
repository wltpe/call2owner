using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class VisitingHelpCategoryCompany
{
    public int Id { get; set; }

    public int VisitingHelpCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<ResidentFrequentlyEntry> ResidentFrequentlyEntries { get; set; } = new List<ResidentFrequentlyEntry>();

    public virtual VisitingHelpCategory VisitingHelpCategory { get; set; } = null!;
}
