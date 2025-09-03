using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Refund
{
    public Guid Id { get; set; }

    public string? OrderId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? TransactionId { get; set; }

    public Guid? UserName { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public string? Utrsource { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    public string? Comment { get; set; }

    public virtual User? UserNameNavigation { get; set; }
}
