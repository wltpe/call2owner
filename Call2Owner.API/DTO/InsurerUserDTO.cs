namespace Call2Owner.DTO
{
    public class InsurerUserDTO
    {
        public Guid UserId { get; set; }
        public Guid InsurerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
