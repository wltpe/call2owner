using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class SocietyDocumentUploaded
{
    public Guid Id { get; set; }

    public Guid SocietyId { get; set; }

    public Guid SocietyDocumentRequiredToRegisterId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual Society Society { get; set; } = null!;

    public virtual SocietyDocumentRequiredToRegister SocietyDocumentRequiredToRegister { get; set; } = null!;
}
