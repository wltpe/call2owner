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

    }

    public class ResidentApprovalDto
    {
        public bool IsApproved { get; set; }
        public string ApprovedComment { get; set; }
    }
}
