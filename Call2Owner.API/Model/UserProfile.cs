using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? ProfileJson { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}
