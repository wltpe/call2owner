using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class AppConfig
{
    public int Id { get; set; }

    public string? ConfigType { get; set; }

    public int? ConfigTypeValue { get; set; }

    public string? ConfigName { get; set; }

    public string? PropertyKey { get; set; }

    public string? PropertyValue { get; set; }

    public string? Attribute { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    public string? PropertyLabel { get; set; }
}
