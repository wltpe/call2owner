using System.ComponentModel.DataAnnotations;

namespace Call2Owner.DTO
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginSelfDto
    {
        public string UserName { get; set; }
        public string OTP { get; set; }
    }
}
