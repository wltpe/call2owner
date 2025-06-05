namespace Call2Owner.DTO
{
    public class SocietyDocumentUploadedDTO
    {
        public Guid Id { get; set; }
        public Guid SocietyId { get; set; }
        public Guid SocietyDocumentRequiredToRegisterId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
}
