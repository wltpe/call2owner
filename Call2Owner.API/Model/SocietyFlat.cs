using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class SocietyFlat
{
    public int Id { get; set; }

    public int SocietyId { get; set; }

    public int? SocietyBuildingId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string FlatImage { get; set; } = null!;

    public bool IsFavourite { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string DeletedBy { get; set; } = null!;

    public DateTime? DeletedOn { get; set; }
}
