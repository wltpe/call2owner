using System;
using System.Collections.Generic;

namespace Call2Owner.Models;
public class SocietyBuildingType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? ParentId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsTimeSlotRequired { get; set; }

    public string? TimeSlotJson { get; set; }

    public virtual ICollection<SocietyUserProfile> SocietyUserProfile { get; set; } = new List<SocietyUserProfile>();
}
