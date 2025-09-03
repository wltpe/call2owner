using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class PlanCircleCode
{
    public int PlanCircleCodeId { get; set; }

    public int PlanProviderId { get; set; }

    public string Circle { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; }

    public int? CircleMasterId { get; set; }

    public virtual ICollection<CircleCode> CircleCodeMapCircle1Navigations { get; set; } = new List<CircleCode>();

    public virtual ICollection<CircleCode> CircleCodeMapCircle2Navigations { get; set; } = new List<CircleCode>();

    public virtual ICollection<CircleCode> CircleCodeMapCircle3Navigations { get; set; } = new List<CircleCode>();

    public virtual CircleMaster? CircleMaster { get; set; }

    public virtual PlanProvider PlanProvider { get; set; } = null!;
}
