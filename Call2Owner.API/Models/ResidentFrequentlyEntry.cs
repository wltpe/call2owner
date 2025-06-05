using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class ResidentFrequentlyEntry
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }

    public string EntryType { get; set; } = null!;

    public string FrequentlyType { get; set; } = null!;

    public string? AllowEntryInNext { get; set; }

    public DateOnly? EntryDate { get; set; }

    public bool? IsSurpriseDelivery { get; set; }

    public bool? IsLeaveAtGate { get; set; }

    public TimeOnly EntryTimeStart { get; set; }

    public TimeOnly EntryTimeEnd { get; set; }

    public string? VehicleNo { get; set; }

    public int? CabCompanyId { get; set; }

    public int? DeliveryCompanyId { get; set; }

    public int? VisitingHelpCategoryCompanyId { get; set; }

    public string? DaysOfWeek { get; set; }

    public string? Validity { get; set; }

    public string? EntriesPerDay { get; set; }

    public string UniqueEntryCode { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual CabCompany? CabCompany { get; set; }

    public virtual DeliveryCompany? DeliveryCompany { get; set; }

    public virtual Resident Resident { get; set; } = null!;

    public virtual VisitingHelpCategoryCompany? VisitingHelpCategoryCompany { get; set; }
}
