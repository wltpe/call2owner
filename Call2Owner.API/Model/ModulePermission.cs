using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class ModulePermission
{
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public string PermissionsJson { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
