using System;
using System.Collections.Generic;

namespace db_test.Models;

public partial class User
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public int? EntityTypeDetailId { get; set; }

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

    public string CreatedBy { get; set; } = null!;

    public DateTime? CreatedOn { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserParent> UserParents { get; set; } = new List<UserParent>();

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
}
