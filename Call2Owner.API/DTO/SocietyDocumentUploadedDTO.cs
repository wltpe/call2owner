namespace Oversight.DTO
{
    public class SocietyDocumentUploadedDTO
    {
        public int Id { get; set; }
        public int SocietyId { get; set; }
        public int SocietyDocumentRequiredToRegisterId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
}
