using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class UserFavorite
{
    public int Id { get; set; }

    public Guid? UserName { get; set; }

    public int ServiceTypeId { get; set; }

    public bool IsActive { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual User? UserNameNavigation { get; set; }
}
