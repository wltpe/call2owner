using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();
}
