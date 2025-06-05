using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public int? ParentRoleId { get; set; }

    public string? DisplayName { get; set; }

    public virtual ICollection<Role> InverseParentRole { get; set; } = new List<Role>();

    public virtual Role? ParentRole { get; set; }

    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
