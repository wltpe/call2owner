using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyUserDocumentUploaded
{
    public Guid Id { get; set; }

    public Guid SocietyUserId { get; set; }

    public int EntityTypeDetailId { get; set; }

    public string? Name { get; set; } = null!;

    public string? Value { get; set; }
    public string? DetailJson { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual SocietyUser SocietyUser { get; set; } = null!;

    public virtual EntityTypeDetail EntityTypeDetail { get; set; } = null!;
}
