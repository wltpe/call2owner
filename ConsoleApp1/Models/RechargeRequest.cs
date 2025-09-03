using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class RechargeRequest
{
    public Guid RechargeRequestId { get; set; }

    public DateTime? RechargeTime { get; set; }

    public DateTime? StatusUpdatedOnTime { get; set; }

    public string? Status { get; set; }

    public string? OrderId { get; set; }

    public int? ProviderId { get; set; }

    public string? Circle { get; set; }

    public string? TransactionId { get; set; }

    public string? Operator { get; set; }

    public string? Number { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Commission { get; set; }

    public decimal? Balance { get; set; }

    public string? ApiRequest { get; set; }

    public string? ApiResponse { get; set; }

    public string? ApiRequestPayload { get; set; }

    public string? ApiHeader { get; set; }

    public string? Message { get; set; }

    public string? OperatorId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    public Guid? UserName { get; set; }

    public decimal? PaymentGatewayAmount { get; set; }

    public decimal? WalletAmount { get; set; }

    public int? OperatorMasterServiceTypeId { get; set; }

    public Guid? CustomerWalletId { get; set; }

    public Guid? GatewayTransactionId { get; set; }

    public int? StatusUpdateCount { get; set; }

    public Guid? AdminWalletId { get; set; }

    public string? ProviderOrderId { get; set; }

    public string? ProviderOperatorId { get; set; }

    public decimal? AdminMarginValue { get; set; }

    public decimal? CustomerDiscountValue { get; set; }

    public bool? IsDisputed { get; set; }

    public bool? IsResolved { get; set; }

    public string? RechargeVerbType { get; set; }

    public Guid? CustomerWalletSecondaryId { get; set; }

    public bool? IsSurchargeApplicable { get; set; }

    public bool? IsSurchargeInPercentage { get; set; }

    public bool? IsTaxApplicable { get; set; }

    public bool? IsTaxInPercentage { get; set; }

    public bool? IsTaxInclusive { get; set; }

    public decimal? SurchargeAmount { get; set; }

    public decimal? SurchargeValue { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? TaxValue { get; set; }

    public virtual AdminWallet? AdminWallet { get; set; }

    public virtual CustomerWallet? CustomerWallet { get; set; }

    public virtual CustomerWallet? CustomerWalletSecondary { get; set; }

    public virtual GatewayTransaction? GatewayTransaction { get; set; }

    public virtual OperatorMasterServiceType? OperatorMasterServiceType { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual ICollection<RechargeRequestDispute> RechargeRequestDisputes { get; set; } = new List<RechargeRequestDispute>();

    public virtual User? UserNameNavigation { get; set; }
}
