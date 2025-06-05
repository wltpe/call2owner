using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class ResidentPet
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public string PetType { get; set; } = null!;

    public string PetBreed { get; set; } = null!;

    public string PetName { get; set; } = null!;

    public string VaccinationType { get; set; } = null!;

    public DateOnly? DateOfVaccination { get; set; }

    public DateOnly? NextVaccinationDueOn { get; set; }

    public bool? RemindMe { get; set; }

    public string? VaccinationDoc { get; set; }

    public string? PetPicture { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual Resident Resident { get; set; } = null!;
}
