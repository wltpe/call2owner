using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class UserSubscriptionPlan
{
    public Guid? UserName { get; set; }

    public string? OrderId { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? DateOfJoining { get; set; }

    public DateTime? PlanExpiryDate { get; set; }

    public bool? IsAdminMarginApplicable { get; set; }

    public bool? IsAdminMarginInPercentage { get; set; }

    public decimal? AdminMarginValue { get; set; }

    public int? DiscountType { get; set; }

    public bool? IsDiscountApplicable { get; set; }

    public bool? IsDiscountInPercentage { get; set; }

    public decimal? DiscountValue { get; set; }

    public decimal? WalletAmount { get; set; }

    public decimal? PaymentGatewayAmount { get; set; }

    public string? RechargeVerbType { get; set; }

    public bool? IsDisputed { get; set; }

    public bool? IsResolved { get; set; }

    public string? Optional1 { get; set; }

    public string? Optional2 { get; set; }

    public string? Optional3 { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsActive { get; set; }

    public Guid? GatewayTransactionId { get; set; }

    public Guid? CustomerWalletId { get; set; }

    public Guid? AdminWalletId { get; set; }

    public int? SubscriptionPlanId { get; set; }

    public int? OperatorMasterServiceTypeId { get; set; }

    public bool? IsUserProfileActivated { get; set; }

    public bool? IsUserWhitelisted { get; set; }

    public string? Status { get; set; }

    public Guid Id { get; set; }

    public virtual AdminWallet? AdminWallet { get; set; }

    public virtual CustomerWallet? CustomerWallet { get; set; }

    public virtual GatewayTransaction? GatewayTransaction { get; set; }

    public virtual OperatorMasterServiceType? OperatorMasterServiceType { get; set; }

    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
}
