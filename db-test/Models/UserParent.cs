using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class UserParent
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int ParentId { get; set; }

    public bool? IsVerified { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual User User { get; set; } = null!;
}
