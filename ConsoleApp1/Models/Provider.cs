using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Provider
{
    public int ProviderId { get; set; }

    public string ProviderName { get; set; } = null!;

    public string ProviderDetail { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<AdminWallet> AdminWallets { get; set; } = new List<AdminWallet>();

    public virtual ICollection<CircleCode> CircleCodes { get; set; } = new List<CircleCode>();

    public virtual ICollection<ErrorCode> ErrorCodes { get; set; } = new List<ErrorCode>();

    public virtual ICollection<OperatorCode> OperatorCodes { get; set; } = new List<OperatorCode>();

    public virtual ICollection<RechargeRequestDispute> RechargeRequestDisputes { get; set; } = new List<RechargeRequestDispute>();

    public virtual ICollection<RechargeRequest> RechargeRequests { get; set; } = new List<RechargeRequest>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
