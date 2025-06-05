using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class ResidentFamily
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public string FamilyType { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public string Name { get; set; } = null!;

    public string? MobileNumber { get; set; }

    public string? ExitType { get; set; }

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
