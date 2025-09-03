using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class OperatorMaster
{
    public int OperatorMasterId { get; set; }

    public string OperatorName { get; set; } = null!;

    public string OperatorMasterCode { get; set; } = null!;

    public string? MobileImage { get; set; }

    public string? WebImage { get; set; }

    public string? OptionalImage { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OperatorMasterServiceType> OperatorMasterServiceTypes { get; set; } = new List<OperatorMasterServiceType>();
}
