using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class City
{
    public int Id { get; set; }

    public int StateId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CityImage { get; set; }

    public bool IsFavourite { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<Society> Society { get; set; } = new List<Society>();

    public virtual State State { get; set; } = null!;
}
