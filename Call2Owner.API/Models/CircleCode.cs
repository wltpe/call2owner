using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class CircleCode
{
    public int CircleCodeId { get; set; }

    public string Circle { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? MapCircle1 { get; set; }

    public int? MapCircle2 { get; set; }

    public int? MapCircle3 { get; set; }

    public bool IsActive { get; set; }

    public int ProviderId { get; set; }

    public virtual PlanCircleCode? MapCircle1Navigation { get; set; }

    public virtual PlanCircleCode? MapCircle2Navigation { get; set; }

    public virtual PlanCircleCode? MapCircle3Navigation { get; set; }

    public virtual Provider Provider { get; set; } = null!;
}
