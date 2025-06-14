using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class EntityTypeDetail
{
    public int Id { get; set; }

    public int EntityTypeId { get; set; }

    public string Value { get; set; } = null!;

    public string Label { get; set; } = null!;

    public string? DetailJson { get; set; }

    public bool IsDafault { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual EntityType EntityType { get; set; } = null!;

    public virtual ICollection<Resident> Resident { get; set; } = new List<Resident>();

    public virtual ICollection<ResidentDocumentUploaded> ResidentDocumentUploaded { get; set; } = new List<ResidentDocumentUploaded>();

    public virtual ICollection<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; } = new List<SocietyDocumentUploaded>();

    public virtual ICollection<SocietyUser> SocietyUser { get; set; } = new List<SocietyUser>();

    public virtual ICollection<SocietyUserDocumentUploaded> SocietyUserDocumentUploaded { get; set; } = new List<SocietyUserDocumentUploaded>();
}
