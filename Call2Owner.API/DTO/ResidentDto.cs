using Call2Owner.Controllers;

namespace Call2Owner.DTO
{
    public class CreateResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }


    }

    public class UpdateResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }
        
        public UploadSelectedRecord? ResidentType { get; set; }

    }
    public class ResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }

    }

    public class ResidentApprovalDto
    {
        public Guid ResidentId { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedComment { get; set; }
    }
}
