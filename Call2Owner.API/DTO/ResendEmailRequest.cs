using System.ComponentModel.DataAnnotations;

namespace Call2Owner.DTO
{
    public class ResendEmailRequest
    {
        [Required]
        [EmailAddress]
        public string MobileNumber { get; set; }
    }
}
