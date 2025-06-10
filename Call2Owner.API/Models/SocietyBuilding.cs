using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyBuilding
{
    public Guid Id { get; set; }

    public Guid SocietyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public string? BuildingImage { get; set; } = null!;

    public bool IsFavourite { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual Society Society { get; set; } = null!;

    public virtual ICollection<SocietyFlat> SocietyFlats { get; set; } = new List<SocietyFlat>();
}
