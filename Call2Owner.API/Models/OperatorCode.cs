using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class OperatorCode
{
    public int OperatorCodeId { get; set; }

    public string OperatorName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int? MapOperator1 { get; set; }

    public int? MapOperator2 { get; set; }

    public int? MapOperator3 { get; set; }

    public bool IsActive { get; set; }

    public int OperatorMasterServiceTypeId { get; set; }

    public int ProviderId { get; set; }

    public int? Priority { get; set; }

    public int? MaxHits { get; set; }

    public decimal? AdminMarginValue { get; set; }

    public int? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public bool? IsAdminMarginApplicable { get; set; }

    public bool? IsAdminMarginInPercentage { get; set; }

    public bool? IsDiscountApplicable { get; set; }

    public bool? IsDiscountInPercentage { get; set; }

    public int? MapOperator4 { get; set; }

    public int? MapOperator5 { get; set; }

    public string? RequestParameterDetails { get; set; }

    public string? CustomerDiscountCalculateByMargin { get; set; }

    public bool? IsRound { get; set; }

    public virtual PlanOperatorCode? MapOperator1Navigation { get; set; }

    public virtual PlanOperatorCode? MapOperator2Navigation { get; set; }

    public virtual PlanOperatorCode? MapOperator3Navigation { get; set; }

    public virtual PlanOperatorCode? MapOperator4Navigation { get; set; }

    public virtual PlanOperatorCode? MapOperator5Navigation { get; set; }

    public virtual OperatorMasterServiceType OperatorMasterServiceType { get; set; } = null!;

    public virtual Provider Provider { get; set; } = null!;
}
