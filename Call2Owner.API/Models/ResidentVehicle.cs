using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ResidentVehicle
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public string VehicleName { get; set; } = null!;

    public string VehicleNumber { get; set; } = null!;

    public string VehicleType { get; set; } = null!;

    public string FuelType { get; set; } = null!;

    public string? RfidTagNumber { get; set; }

    public string? Code { get; set; }

    public bool NotifyMeOnEntryExit { get; set; }

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
