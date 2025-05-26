namespace Oversight.DTO
{
    public class SocietyDocumentRequiredToRegisterDTO
    {
        public int Id { get; set; }
        public int EntityTypeDetailId { get; set; }
        public string Value { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
    }
}
