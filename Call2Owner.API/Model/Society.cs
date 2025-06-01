using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class Society
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public int StateId { get; set; }

    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string SocietyImage { get; set; } = null!;

    public bool IsSuggested { get; set; }

    public string SuggestedBy { get; set; } = null!;

    public DateTime? SuggestedOn { get; set; }

    public int? EntityTypeDetailId { get; set; }

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

    public string DeletedBy { get; set; } = null!;

    public DateTime? DeletedOn { get; set; }
}
