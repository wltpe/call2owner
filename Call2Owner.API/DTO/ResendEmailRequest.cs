using System.ComponentModel.DataAnnotations;

namespace Call2Owner.DTO
{
    public class ResendEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
