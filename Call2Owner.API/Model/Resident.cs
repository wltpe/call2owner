using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class Resident
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SocietyFlatId { get; set; }

    public int? EntityTypeDetailId { get; set; }

    public bool IsDocumentUploaded { get; set; }

    public bool IsApproved { get; set; }

    public string ApprovedBy { get; set; } = null!;

    public DateTime? ApprovedOn { get; set; }

    public string ApprovedComment { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
}
