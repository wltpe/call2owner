using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class GatewayTransaction
{
    public Guid Id { get; set; }

    public string? OrderId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? TransactionType { get; set; }

    public string? ResponseCode { get; set; }

    public string? ResponseDescription { get; set; }

    public decimal? Amount { get; set; }

    public string? PgTransId { get; set; }

    public string? PgTransTime { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public string? Status { get; set; }

    public Guid? UserName { get; set; }

    public string? PaymentMode { get; set; }

    public string? TxnStatus { get; set; }

    public string? ApSecureHash { get; set; }

    public string? BankResponseMsg { get; set; }

    public string? BilledAmount { get; set; }

    public string? CurrencyCode { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerVpa { get; set; }

    public string? Message { get; set; }

    public string? PgTransMode { get; set; }

    public string? Risk { get; set; }

    public string? TransactionCategory { get; set; }

    public string? Comment { get; set; }

    public string? BankTransId { get; set; }

    public string? Number { get; set; }

    public string? ServiceType { get; set; }

    public virtual ICollection<CustomerWallet> CustomerWallets { get; set; } = new List<CustomerWallet>();

    public virtual ICollection<RechargeRequest> RechargeRequests { get; set; } = new List<RechargeRequest>();

    public virtual User? UserNameNavigation { get; set; }

    public virtual ICollection<UserSubscriptionPlan> UserSubscriptionPlans { get; set; } = new List<UserSubscriptionPlan>();
}
