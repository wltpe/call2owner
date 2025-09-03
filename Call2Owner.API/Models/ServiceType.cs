using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ServiceType
{
    public int ServiceTypeId { get; set; }

    public int ServiceCategoryId { get; set; }

    public string? ServiceName { get; set; }

    public string? ServiceLabel { get; set; }

    public string? Code { get; set; }

    public string? MobileImage { get; set; }

    public string? WebImage { get; set; }

    public string? OptionalImage { get; set; }

    public bool IsActive { get; set; }

    public int? DisplayOrder { get; set; }

    public bool? Recommended { get; set; }

    public int? PlanProviderId { get; set; }

    public string? ApiVersion { get; set; }

    public string? PaybackMode { get; set; }

    public int? PgId { get; set; }

    public bool WalletApplicable { get; set; }

    public string? PaymentParameterDetails { get; set; }

    public virtual ICollection<OperatorMasterServiceType> OperatorMasterServiceTypes { get; set; } = new List<OperatorMasterServiceType>();

    public virtual PlanProvider? PlanProvider { get; set; }

    public virtual ServiceCategory ServiceCategory { get; set; } = null!;

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
