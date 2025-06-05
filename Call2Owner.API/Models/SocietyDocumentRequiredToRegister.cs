using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyDocumentRequiredToRegister
{
    public Guid Id { get; set; }

    public int EntityTypeDetailId { get; set; }

    public string Value { get; set; } = null!;

    public bool IsRequired { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual EntityTypeDetail EntityTypeDetail { get; set; } = null!;

    public virtual ICollection<SocietyDocumentUploaded> SocietyDocumentUploadeds { get; set; } = new List<SocietyDocumentUploaded>();
}
