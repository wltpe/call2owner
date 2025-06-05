using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Permissions
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;
}
