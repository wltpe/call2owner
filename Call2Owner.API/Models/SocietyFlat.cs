using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyFlat
{
    public Guid Id { get; set; }

    public Guid SocietyId { get; set; }

    public Guid SocietyBuildingId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? FlatImage { get; set; }

    public bool IsFavourite { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<Resident> Resident { get; set; } = new List<Resident>();

    public virtual Society Society { get; set; } = null!;

    public virtual SocietyBuilding SocietyBuilding { get; set; } = null!;
}
