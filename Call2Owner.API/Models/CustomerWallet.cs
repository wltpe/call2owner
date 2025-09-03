using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class CustomerWallet
{
    public Guid Id { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public string? Detail { get; set; }

    public string? OrderId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public Guid? UserName { get; set; }

    public Guid? GatewayTransactionId { get; set; }

    public string? Comment { get; set; }

    public virtual GatewayTransaction? GatewayTransaction { get; set; }

    public virtual ICollection<RechargeRequest> RechargeRequestCustomerWalletSecondaries { get; set; } = new List<RechargeRequest>();

    public virtual ICollection<RechargeRequest> RechargeRequestCustomerWallets { get; set; } = new List<RechargeRequest>();

    public virtual User? UserNameNavigation { get; set; }

    public virtual ICollection<UserSubscriptionPlan> UserSubscriptionPlans { get; set; } = new List<UserSubscriptionPlan>();
}
