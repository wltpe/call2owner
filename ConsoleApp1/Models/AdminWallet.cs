using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class AdminWallet
{
    public Guid Id { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public string? Detail { get; set; }

    public string? OrderId { get; set; }

    public string? MobileNumber { get; set; }

    public decimal? RechargeAmount { get; set; }

    public decimal? AbsoluteMarginValue { get; set; }

    public string? PaymentMode { get; set; }

    public decimal? PaymentGatewayAmount { get; set; }

    public decimal? AdminMarginValue { get; set; }

    public decimal? CustomerDiscountValue { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public int? ProviderId { get; set; }

    public Guid? UserName { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<RechargeRequest> RechargeRequests { get; set; } = new List<RechargeRequest>();

    public virtual User? UserNameNavigation { get; set; }

    public virtual ICollection<UserSubscriptionPlan> UserSubscriptionPlans { get; set; } = new List<UserSubscriptionPlan>();
}
