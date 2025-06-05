namespace Call2Owner.DTO
{
    public class SocietyDto
    {
        public Guid Id { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SocietyImage { get; set; }
        public bool IsSuggested { get; set; }
        public string SuggestedBy { get; set; }
        public DateTime? SuggestedOn { get; set; }
        public int? EntityTypeDetailId { get; set; }
        public bool IsActive { get; set; }
    }

    public class SocietyApprovalDto
    {
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedComment { get; set; }
    }
}
