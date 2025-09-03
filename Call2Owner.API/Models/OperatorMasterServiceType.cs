using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class OperatorMasterServiceType
{
    public int OperatorMasterServiceTypeId { get; set; }

    public int ServiceTypeId { get; set; }

    public string OperatorName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string MobileImage { get; set; } = null!;

    public string WebImage { get; set; } = null!;

    public string OptionalImage { get; set; } = null!;

    public bool IsActive { get; set; }

    public int OperatorMasterId { get; set; }

    public virtual ICollection<OperatorCode> OperatorCodes { get; set; } = new List<OperatorCode>();

    public virtual OperatorMaster OperatorMaster { get; set; } = null!;

    public virtual ICollection<PlanOperatorCode> PlanOperatorCodes { get; set; } = new List<PlanOperatorCode>();

    public virtual ICollection<RechargeRequestDispute> RechargeRequestDisputes { get; set; } = new List<RechargeRequestDispute>();

    public virtual ICollection<RechargeRequest> RechargeRequests { get; set; } = new List<RechargeRequest>();

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual ICollection<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();

    public virtual ICollection<UserSubscriptionPlan> UserSubscriptionPlans { get; set; } = new List<UserSubscriptionPlan>();
}
