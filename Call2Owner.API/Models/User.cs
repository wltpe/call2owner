using System;
using System.Collections.Generic;

namespace Call2Owner.Models;

public partial class User
{
    public Guid UserName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpExpireTime { get; set; }

    public string Role { get; set; } = null!;

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? AccessTokenExpireTime { get; set; }

    public DateTime? RefreshTokenExpireTime { get; set; }

    public DateTime? ResendOtpTime { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? OtpValidatedOn { get; set; }

    public string? Address { get; set; }

    public bool? IsKycaadhaarVerified { get; set; }

    public bool? IsPremiumCustomer { get; set; }

    public string? ProfileImage { get; set; }

    public string? Hash { get; set; }

    public string? Salt { get; set; }

    public string? Dob { get; set; }

    public bool? IsKycPanVerified { get; set; }

    public DateTime? IsKycPanVerifiedDate { get; set; }

    public DateTime? IsKycaadhaarVerifiedDate { get; set; }

    public string? Plateform { get; set; }

    public string? UserAgent { get; set; }

    public string? AadhaarNumber { get; set; }

    public string? Gender { get; set; }

    public bool? IsKycCompleted { get; set; }

    public string? MiddleName { get; set; }

    public string? PanNumber { get; set; }

    public string? Prefix { get; set; }

    public string? CreatedBy { get; set; }

    public int RolesId { get; set; }

    public string? UpdatedBy { get; set; }

    public string? PasswordHash { get; set; }
    public bool? IsVerified { get; set; }
    public string? VerificationCode { get; set; }

    public DateTime? VerificationCodeGenerationTime { get; set; }

    public DateTime? VerificationCodeValidationTime { get; set; }

    public virtual ICollection<AdminWallet> AdminWallets { get; set; } = new List<AdminWallet>();

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    public virtual ICollection<CustomerWallet> CustomerWallets { get; set; } = new List<CustomerWallet>();

    public virtual ICollection<GatewayTransaction> GatewayTransactions { get; set; } = new List<GatewayTransaction>();

    public virtual ICollection<RechargeRequestDispute> RechargeRequestDisputes { get; set; } = new List<RechargeRequestDispute>();

    public virtual ICollection<RechargeRequest> RechargeRequests { get; set; } = new List<RechargeRequest>();

    public virtual ICollection<Refund> Refund { get; set; } = new List<Refund>();

    public virtual ICollection<Resident> Resident { get; set; } = new List<Resident>();

    public virtual Role Roles { get; set; } = null!;

    public virtual ICollection<SocietyUser> SocietyUser { get; set; } = new List<SocietyUser>();

    public virtual ICollection<UserFavorite> UserFavorite { get; set; } = new List<UserFavorite>();

    public virtual ICollection<UserParent> UserParent { get; set; } = new List<UserParent>();

    public virtual ICollection<UserProfile> UserProfile { get; set; } = new List<UserProfile>();
}
