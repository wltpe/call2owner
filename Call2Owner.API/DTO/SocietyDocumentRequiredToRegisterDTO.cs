namespace Call2Owner.DTO
{
    public class SocietyDocumentRequiredToRegisterDTO
    {
        public Guid Id { get; set; }
        public Guid EntityTypeDetailId { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
    }
}
