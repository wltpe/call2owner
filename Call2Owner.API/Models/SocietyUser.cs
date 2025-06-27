using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyUser
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? SocietyId { get; set; }

    public int? EntityTypeDetailId { get; set; }

    public bool IsDocumentUploaded { get; set; }

    public bool IsApproved { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string? ApprovedComment { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool? IsDocumentRequired { get; set; }
    public string? DetailJson { get; set; }

    public virtual EntityTypeDetail? EntityTypeDetail { get; set; }

    public virtual Society? Society { get; set; }

    public virtual ICollection<SocietyUserDocumentUploaded> SocietyUserDocumentUploaded { get; set; } = new List<SocietyUserDocumentUploaded>();

    public virtual User User { get; set; } = null!;
}
