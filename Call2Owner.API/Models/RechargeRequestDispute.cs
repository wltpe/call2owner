using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class RechargeRequestDispute
{
    public Guid RechargeRequestDisputeId { get; set; }

    public DateTime? DisputeTime { get; set; }

    public DateTime? StatusUpdatedOnTime { get; set; }

    public string? Status { get; set; }

    public string? OrderId { get; set; }

    public string? TransactionId { get; set; }

    public string? Circle { get; set; }

    public string? Operator { get; set; }

    public string? Number { get; set; }

    public decimal? Amount { get; set; }

    public string? Message { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public int? OperatorMasterServiceTypeId { get; set; }

    public Guid? UserName { get; set; }

    public Guid? RechargeRequestId { get; set; }

    public int? ProviderId { get; set; }

    public bool? IsDisputed { get; set; }

    public bool? IsResolved { get; set; }

    public virtual OperatorMasterServiceType? OperatorMasterServiceType { get; set; }

    public virtual Provider? Provider { get; set; }

    public virtual RechargeRequest? RechargeRequest { get; set; }

    public virtual User? UserNameNavigation { get; set; }
}
