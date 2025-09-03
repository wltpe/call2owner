using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();
}
