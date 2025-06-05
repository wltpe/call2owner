namespace Call2Owner.DTO
{
    public class UsersDtoOutput
    {
        public Guid Id { get; set; }
        public Guid userId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }
    }
}
