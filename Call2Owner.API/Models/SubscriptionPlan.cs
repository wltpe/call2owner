using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SubscriptionPlan
{
    public int SubscriptionPlanId { get; set; }

    public int PlanId { get; set; }

    public string? PlanName { get; set; }

    public string? PlanTitle { get; set; }

    public string? PlanDescription { get; set; }

    public string? MobileImage { get; set; }

    public string? WebImage { get; set; }

    public decimal? Amount { get; set; }

    public bool? IsAdminMarginApplicable { get; set; }

    public bool? IsAdminMarginInPercentage { get; set; }

    public decimal? AdminMarginValue { get; set; }

    public int? DiscountType { get; set; }

    public bool? IsDiscountApplicable { get; set; }

    public bool? IsDiscountInPercentage { get; set; }

    public decimal? DiscountValue { get; set; }

    public int? PlanExpiryInDays { get; set; }

    public string? Optional1 { get; set; }

    public string? Optional2 { get; set; }

    public string? Optional3 { get; set; }

    public bool? IsActive { get; set; }

    public int? OperatorMasterServiceTypeId { get; set; }

    public virtual OperatorMasterServiceType? OperatorMasterServiceType { get; set; }

    public virtual ICollection<UserSubscriptionPlan> UserSubscriptionPlans { get; set; } = new List<UserSubscriptionPlan>();
}
