using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Resident
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? SocietyFlatId { get; set; }

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

    public virtual EntityTypeDetail? EntityTypeDetail { get; set; }

    public virtual ICollection<ResidentDocumentUploaded> ResidentDocumentUploaded { get; set; } = new List<ResidentDocumentUploaded>();

    public virtual ICollection<ResidentFamily> ResidentFamily { get; set; } = new List<ResidentFamily>();

    public virtual ICollection<ResidentFrequentlyEntry> ResidentFrequentlyEntry { get; set; } = new List<ResidentFrequentlyEntry>();

    public virtual ICollection<ResidentFrequentlyGuest> ResidentFrequentlyGuest { get; set; } = new List<ResidentFrequentlyGuest>();

    public virtual ICollection<ResidentPet> ResidentPet { get; set; } = new List<ResidentPet>();

    public virtual ICollection<ResidentVehicle> ResidentVehicle { get; set; } = new List<ResidentVehicle>();

    public virtual SocietyFlat? SocietyFlat { get; set; }

    public virtual User User { get; set; } = null!;
}
