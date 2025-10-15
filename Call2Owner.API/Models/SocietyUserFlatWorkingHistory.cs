using System;
using System.Collections.Generic;

namespace Call2Owner.Models;
public class SocietyUserFlatWorkingHistory
{
    public Guid Id { get; set; }

    public Guid SocietyUserProfileId { get; set; }

    public Guid SocietyFlatId { get; set; }

    public bool IsAddedByResident { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public bool IsNotify { get; set; }

    public int? Ratings { get; set; }

    public virtual ICollection<ResidentDailyHelp> ResidentDailyHelp { get; set; } = new List<ResidentDailyHelp>();

    public virtual SocietyFlat SocietyFlat { get; set; } = null!;

    public virtual SocietyUserProfile SocietyUserProfile { get; set; } = null!;
}