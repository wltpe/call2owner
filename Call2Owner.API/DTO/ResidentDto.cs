using Call2Owner.Controllers;
using Call2Owner.Models;

namespace Call2Owner.DTO
{
    public class CreateResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }


    }
    public class UpdateResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }
        
        public UploadSelectedRecord? ResidentType { get; set; }

    }
    public class ResidentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid? SocietyFlatId { get; set; }

        public int? EntityTypeDetailId { get; set; }

        public bool IsDocumentUploaded { get; set; }

        public bool IsApproved { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }

        public string? ApprovedComment { get; set; }

        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }
        public string? DetailJson { get; set; }

    }
    public class ResidentApprovalDto
    {
        public Guid ResidentId { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedComment { get; set; }
    }
    public class ResidentFamilyDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string FamilyType { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public string Name { get; set; } = null!;

        public string? MobileNumber { get; set; }

        public string? ExitType { get; set; }
        public string ResidentFamilyCode { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    public class AddResidentFamilyDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string FamilyType { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public string Name { get; set; } = null!;

        public string? MobileNumber { get; set; }

        public string? ExitType { get; set; }
        public string ResidentFamilyCode { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    public class UpdateResidentFamilyDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string FamilyType { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public string Name { get; set; } = null!;

        public string? MobileNumber { get; set; }

        public string? ExitType { get; set; }
        public string ResidentFamilyCode { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    public class ResidentPetDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string PetType { get; set; } = null!;

        public string PetBreed { get; set; } = null!;

        public string PetName { get; set; } = null!;

        public string? VaccinationType { get; set; } = null!;

        public string? PetPicture { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    public class AddResidentPetDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string PetType { get; set; } = null!;

        public string PetBreed { get; set; } = null!;

        public string PetName { get; set; } = null!;

        public string? VaccinationType { get; set; } = null!;

        public string? PetPicture { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }



    

    

    }
    public class UpdateResidentPetDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string PetType { get; set; } = null!;

        public string PetBreed { get; set; } = null!;

        public string PetName { get; set; } = null!;

        public string? VaccinationType { get; set; } = null!;

        public string? PetPicture { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }

    public class ResidentVehicleDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string VehicleName { get; set; } = null!;

        public string VehicleNumber { get; set; } = null!;

        public string VehicleType { get; set; } = null!;

        public string FuelType { get; set; } = null!;

        public string? RfidTagNumber { get; set; }
        public string? VehiclePicture { get; set; }

        public string? Code { get; set; }

        public bool NotifyMeOnEntryExit { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
    public class AddResidentVehicleDto
    {
        public Guid Id { get; set; }

        public Guid ResidentId { get; set; }

        public string VehicleName { get; set; } = null!;

        public string VehicleNumber { get; set; } = null!;

        public string VehicleType { get; set; } = null!;

        public string FuelType { get; set; } = null!;

        public string? RfidTagNumber { get; set; }
        public string? VehiclePicture { get; set; }

        public string? Code { get; set; }

        public bool NotifyMeOnEntryExit { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }

    public class UpdateResidentVehicleDto
    {
        public Guid ResidentId { get; set; }

        public string VehicleName { get; set; } = null!;

        public string VehicleNumber { get; set; } = null!;

        public string VehicleType { get; set; } = null!;

        public string FuelType { get; set; } = null!;

        public string? RfidTagNumber { get; set; }
        public string? VehiclePicture { get; set; }

        public string? Code { get; set; }

        public bool NotifyMeOnEntryExit { get; set; }

        public bool IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
}
