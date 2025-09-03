using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class PlanOperatorCode
{
    public int PlanOperatorCodeId { get; set; }

    public int PlanProviderId { get; set; }

    public string PlanOperator { get; set; } = null!;

    public string PlanCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public int OperatorMasterServiceTypeId { get; set; }

    public string? RequestParameterDetails { get; set; }

    public virtual ICollection<OperatorCode> OperatorCodeMapOperator1Navigations { get; set; } = new List<OperatorCode>();

    public virtual ICollection<OperatorCode> OperatorCodeMapOperator2Navigations { get; set; } = new List<OperatorCode>();

    public virtual ICollection<OperatorCode> OperatorCodeMapOperator3Navigations { get; set; } = new List<OperatorCode>();

    public virtual ICollection<OperatorCode> OperatorCodeMapOperator4Navigations { get; set; } = new List<OperatorCode>();

    public virtual ICollection<OperatorCode> OperatorCodeMapOperator5Navigations { get; set; } = new List<OperatorCode>();

    public virtual OperatorMasterServiceType OperatorMasterServiceType { get; set; } = null!;

    public virtual PlanProvider PlanProvider { get; set; } = null!;
}
