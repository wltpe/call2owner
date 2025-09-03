using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SystemInfo
{
    public Guid Id { get; set; }

    public string IosAppVersion { get; set; } = null!;

    public string ApiVersion { get; set; } = null!;

    public bool IsSoftUpdate { get; set; }

    public bool IsHardUpdate { get; set; }

    public string? Message { get; set; }

    public bool IsActive { get; set; }

    public bool IsMaintenanceModeActive { get; set; }

    public string AndroidAppVersion { get; set; } = null!;
}
