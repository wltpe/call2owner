using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public class SocietyUserTimeSlot
{
    public Guid Id { get; set; }

    public Guid SocietyUserProfileId { get; set; }

    public string? AllowedTimeSlotJson { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual SocietyUserProfile SocietyUserProfile { get; set; } = null!;
}

