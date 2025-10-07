using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class SocietyUserProfile
{
    public Guid Id { get; set; }

    public Guid SocietyUserId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? Name { get; set; }

    public string? AadhaarNumber { get; set; }

    public string? PanNumber { get; set; }

    public string? AadhaarDocumentPic1 { get; set; }

    public string? AadhaarDocumentPic2 { get; set; }

    public string? PanDocumentPic { get; set; }

    public string? ProfilePic { get; set; }

    public int? SocietyBuildingTypeId { get; set; }

    public string? UniqueCode { get; set; }

    public string? VehicleNo { get; set; }

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DetailJson { get; set; }
    public bool? IsInside { get; set; }

    public virtual SocietyBuildingType? SocietyBuildingType { get; set; }

    public virtual SocietyUser SocietyUser { get; set; } = null!;

    public virtual ICollection<SocietyUserFlatWorkingHistory> SocietyUserFlatWorkingHistory { get; set; } = new List<SocietyUserFlatWorkingHistory>();

    public virtual ICollection<SocietyUserTimeSlot> SocietyUserTimeSlot { get; set; } = new List<SocietyUserTimeSlot>();
}
