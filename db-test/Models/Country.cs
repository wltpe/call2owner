using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CountryImage { get; set; }

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

    public virtual ICollection<Society> Societies { get; set; } = new List<Society>();

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
