using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class UserReferralCode
{
    public Guid Id { get; set; }

    public Guid? Username { get; set; }

    public string? ReferralCode { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? CashbackAmount { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }
}
