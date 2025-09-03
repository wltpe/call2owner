using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class CircleMaster
{
    public int CircleMasterId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<PlanCircleCode> PlanCircleCodes { get; set; } = new List<PlanCircleCode>();
}
