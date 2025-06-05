using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ModulePermission
{
    public int Id { get; set; }

    public int ModuleId { get; set; }

    public string PermissionsJson { get; set; } = null!;

    public virtual Modules Module { get; set; } = null!;
}
