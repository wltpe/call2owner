using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();
}
