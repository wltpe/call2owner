using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Beneficiary
{
    public Guid Id { get; set; }

    public Guid? UserName { get; set; }

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public string? NickName { get; set; }

    public string? IfscCode { get; set; }

    public string? PanNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    public string? Comment { get; set; }

    public string? AccountName { get; set; }

    public string? AccountNumber { get; set; }

    public decimal? Amount { get; set; }

    public virtual User? UserNameNavigation { get; set; }
}
