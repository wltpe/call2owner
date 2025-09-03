using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class PlanProvider
{
    public int PlanProviderId { get; set; }

    public string PlanProviderName { get; set; } = null!;

    public string PlanProviderDetail { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<PlanCircleCode> PlanCircleCodes { get; set; } = new List<PlanCircleCode>();

    public virtual ICollection<PlanOperatorCode> PlanOperatorCodes { get; set; } = new List<PlanOperatorCode>();

    public virtual ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
}
