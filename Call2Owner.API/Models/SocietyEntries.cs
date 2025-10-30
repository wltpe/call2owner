using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Call2Owner.Models;

[Table("SocietyEntries")]
public class SocietyEntries
{
    [Key]
    public Guid Id { get; set; }

    // 🔗 Foreign Keys
    [Required]
    public Guid SocietyFlatId { get; set; }

    [ForeignKey(nameof(SocietyFlatId))]
    public virtual SocietyFlat SocietyFlat { get; set; } = null!;

    public Guid? ResidentFamilyId { get; set; }
    [ForeignKey(nameof(ResidentFamilyId))]
    public virtual ResidentFamily? ResidentFamily { get; set; }

    public Guid? ResidentPetId { get; set; }
    [ForeignKey(nameof(ResidentPetId))]
    public virtual ResidentPet? ResidentPet { get; set; }

    public Guid? ResidentDailyHelpId { get; set; }
    [ForeignKey(nameof(ResidentDailyHelpId))]
    public virtual ResidentDailyHelp? ResidentDailyHelp { get; set; }

    public Guid? ResidentVehicleId { get; set; }
    [ForeignKey(nameof(ResidentVehicleId))]
    public virtual ResidentVehicle? ResidentVehicle { get; set; }

    public Guid? ResidentFrequentlyGuestId { get; set; }
    [ForeignKey(nameof(ResidentFrequentlyGuestId))]
    public virtual ResidentFrequentlyGuest? ResidentFrequentlyGuest { get; set; }

    public Guid? ResidentFrequentlyEntryId { get; set; }
    [ForeignKey(nameof(ResidentFrequentlyEntryId))]
    public virtual ResidentFrequentlyEntry? ResidentFrequentlyEntry { get; set; }

    public int? CabCompanyId { get; set; }
    [ForeignKey(nameof(CabCompanyId))]
    public virtual CabCompany? CabCompany { get; set; }

    public int? DeliveryCompanyId { get; set; }
    [ForeignKey(nameof(DeliveryCompanyId))]
    public virtual DeliveryCompany? DeliveryCompany { get; set; }

    public int? VisitingHelpCategoryId { get; set; }
    [ForeignKey(nameof(VisitingHelpCategoryId))]
    public virtual VisitingHelpCategory? VisitingHelpCategory { get; set; }

    public int? VisitingHelpCategoryCompanyId { get; set; }
    [ForeignKey(nameof(VisitingHelpCategoryCompanyId))]
    public virtual VisitingHelpCategoryCompany? VisitingHelpCategoryCompany { get; set; }

    // 🧍 Entry Details
    [MaxLength(50)]
    public string? MobileNumber { get; set; }

    [MaxLength(50)]
    public string? FullName { get; set; }

    [MaxLength(50)]
    public string? VehicleNumber { get; set; }

    public string? Picture { get; set; }

    [MaxLength(10)]
    public string? UniqueCode { get; set; }

    // 🕓 Entry Tracking
    public string? EntryRegisterBy { get; set; }
    public DateTime? EntryRegisterOn { get; set; }

    public bool? IsEntryApprovalRequiredByResident { get; set; }
    public bool? IsEntryApproved { get; set; }
    public bool? IsEntryApprovalByResident { get; set; }

    public string? EntryApprovedBy { get; set; }
    public DateTime? EntryApprovedOn { get; set; }

    public bool? IsResidentUnableToAnswerForApproval { get; set; }

    [MaxLength(20)]
    public string? ResidentUnableToAnswerForApprovalReason { get; set; }

    public bool? IsEntryDeclined { get; set; }
    public bool? IsEntryDeclinedByResident { get; set; }

    public string? EntryDeclinedBy { get; set; }
    public DateTime? EntryDeclinedOn { get; set; }

    // 🚪 Exit Info
    public bool? IsActive { get; set; }
    public bool? IsInside { get; set; }
    public string? OutBy { get; set; }
    public DateTime? OutOn { get; set; }
    public bool? IsRemoved { get; set; }

    // 🔗 Audit and status
    // (No CreatedBy, UpdatedBy fields in your table — so nothing extra here)
}

