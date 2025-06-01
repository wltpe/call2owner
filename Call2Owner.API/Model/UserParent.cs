using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class UserParent
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ParentId { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public virtual User User { get; set; } = null!;
}
