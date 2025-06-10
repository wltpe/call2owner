using System.ComponentModel.DataAnnotations;

namespace Call2Owner.DTO
{
    public class UserDto
    {
        //public Guid Id { get; set; }
        [Required]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public int RoleId { get; set; }
    }

    public class UserResidentDto
    {
        [Required]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
    }

    public class LoginResidentDto
    {
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
    }
}
