using System;
using System.Collections.Generic;

namespace Call2Owner.API.Model;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public int RoleId { get; set; }

    public string? VerificationCode { get; set; }

    public DateTime? VerificationCodeGenerationTime { get; set; }

    public DateTime? VerificationCodeValidationTime { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpExpireTime { get; set; }

    public DateTime? ResendOtpTime { get; set; }

    public DateTime? OtpValidatedOn { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpireTime { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsVerified { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserParent> UserParents { get; set; } = new List<UserParent>();
}
