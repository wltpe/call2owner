using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ServiceCategory
{
    public int ServiceCategoryId { get; set; }

    public string ServiceCategoryName { get; set; } = null!;

    public string? ServiceCategoryLabel { get; set; }

    public string? Code { get; set; }

    public string? MobileImage { get; set; }

    public string? WebImage { get; set; }

    public string? OptionalImage { get; set; }

    public bool IsActive { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual ICollection<ServiceType> ServiceTypes { get; set; } = new List<ServiceType>();
}
