using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class EntityType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Label { get; set; }

    public string? DafaultValue { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<EntityTypeDetail> EntityTypeDetails { get; set; } = new List<EntityTypeDetail>();
}
