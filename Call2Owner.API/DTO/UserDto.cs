using System.ComponentModel.DataAnnotations;

namespace Call2Owner.DTO
{
    public class UserDto
    {
        //public Guid Id { get; set; }
        [Required]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
        public bool? UsePassword { get; set; } = true;
        [Required]
        public string? MobileNumber { get; set; }
        public int RoleId { get; set; }
    }

    public class SocietyUserDto
    {
        //public Guid Id { get; set; }
        [Required]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? Email { get; set; }

        [Required]
        public string? MobileNumber { get; set; }
        public string? Password { get; set; }

        [Required]
        public int RoleId { get; set; }
        public bool IsDocumentRequired { get; set; }

        [Required]
        public Guid SocietyId { get; set; }
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
