using System;
using System.Collections.Generic;

namespace Call2Owner.Models;
public partial class ResidentDailyHelp
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public Guid SocietyUserFlatWorkingHistoryId { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual Resident Resident { get; set; } = null!;

    public virtual SocietyUserFlatWorkingHistory SocietyUserFlatWorkingHistory { get; set; } = null!;
}
