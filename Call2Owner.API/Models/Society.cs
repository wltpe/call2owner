using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class Society
{
    public Guid Id { get; set; }
    public int CountryId { get; set; }
    public int StateId { get; set; }
    public int CityId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? SocietyImage { get; set; }
    public bool? IsSuggested { get; set; }
    public string? SuggestedBy { get; set; }
    public DateTime? SuggestedOn { get; set; }
    public int? EntityTypeDetailId { get; set; }
    public bool? IsApproved { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string? ApprovedComment { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool? IsDeleted { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Longitude { get; set; }
    public string? Latitude { get; set; }
    public string? PinCode { get; set; }
    public string? Address { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<SocietyBuilding> SocietyBuildings { get; set; } = new List<SocietyBuilding>();

    public virtual ICollection<SocietyDocumentUploaded> SocietyDocumentUploadeds { get; set; } = new List<SocietyDocumentUploaded>();

    public virtual ICollection<SocietyFlat> SocietyFlats { get; set; } = new List<SocietyFlat>();

    public virtual State State { get; set; } = null!;
}
