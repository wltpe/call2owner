using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string PermissionName { get; set; } = null!;
}
