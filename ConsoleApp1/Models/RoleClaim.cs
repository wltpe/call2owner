using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class RoleClaim
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string ModulePermissionsJson { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
