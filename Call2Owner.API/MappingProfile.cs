using AutoMapper;
using Call2Owner.Controllers;
using Call2Owner.DTO;
using Call2Owner.Models;
using Utilities;
using Module = Call2Owner.Models.Module;

namespace Call2Owner
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Module, ModuleDto>().ReverseMap();
            CreateMap<Models.Permission, PermissionDto>().ReverseMap();
            //CreateMap<ModulePermission, ModulePermissionDto>()
            //    .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.ModuleName))
            //    .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.PermissionsJson)).ReverseMap();

            // Mapping from PermissionData to PermissionDataDto
            CreateMap<PermissionData, PermissionDataDto>().ReverseMap();

            // Map for ModulePermission <-> ModulePermissionDto
            CreateMap<ModulePermission, ModulePermissionDto>().ReverseMap();

            CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.ParentRoleId, opt => opt.MapFrom(src => src.ParentRoleId))
            .ForMember(dest => dest.ParentRoleName, opt => opt.MapFrom(src => src.ParentRole != null ? src.ParentRole.RoleName : null)).ReverseMap();


            CreateMap<Role, RoleDetailDto>().ReverseMap();
            CreateMap<RoleClaim, RoleClaimDto>().ReverseMap();

            CreateMap<Role, RoleDetailDto>()
                .ForMember(dest => dest.ParentRoleId, opt => opt.MapFrom(src => src.ParentRoleId))
                .ForMember(dest => dest.ParentRoleName, opt => opt.MapFrom(src => src.ParentRole != null ? src.ParentRole.RoleName : null))
                .ForMember(dest => dest.RoleClaims, opt => opt.MapFrom(src => src.RoleClaim)).ReverseMap();

            CreateMap<Society, SocietyApprovalDto>().ReverseMap();
            CreateMap<Society, SocietyDto>().ReverseMap();
            CreateMap<Society, UpdateSocietyDto>().ReverseMap();

            CreateMap<SocietyDocumentUploaded, SocietyDocumentUploadedDTO>().ReverseMap();

            CreateMap<SocietyFlat, SocietyFlatDTO>().ReverseMap();
            CreateMap<SocietyBuilding, SocietyBuildingDTO>().ReverseMap();

            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<State, StateDto>().ReverseMap();
            CreateMap<City, CityDto>().ReverseMap();

            CreateMap<Resident, ResidentApprovalDto>().ReverseMap();
            CreateMap<Resident, ResidentDto>().ReverseMap();
            CreateMap<Resident, CreateResidentDto>().ReverseMap();
            CreateMap<Resident, UpdateResidentDto>().ReverseMap();

            CreateMap<ResidentFamily, AddResidentFamilyDto>().ReverseMap();
            CreateMap<AddResidentFamily, ResidentFamily>().ReverseMap();
            CreateMap<ResidentFamily, UpdateResidentFamilyDto>().ReverseMap();
            CreateMap<UpdateResidentFamily, ResidentFamily>().ReverseMap();
            CreateMap<ResidentFamily, ResidentFamilyDto>().ReverseMap();

            CreateMap<ResidentPet, AddResidentPetDto>().ReverseMap();
            CreateMap<AddResidentPet, ResidentPet>().ReverseMap();
            CreateMap<ResidentPet, UpdateResidentPetDto>().ReverseMap();
            CreateMap<UpdateResidentPet, ResidentPet>().ReverseMap();
            CreateMap<ResidentPet, ResidentPetDto>().ReverseMap();

            CreateMap<ResidentVehicle, AddResidentVehicleDto>().ReverseMap();
            CreateMap<AddResidentVehicle, ResidentVehicle>().ReverseMap();
            CreateMap<ResidentVehicle, UpdateResidentVehicleDto>().ReverseMap();
            CreateMap<UpdateResidentVehicle, ResidentVehicle>().ReverseMap();
            CreateMap<ResidentVehicle, ResidentVehicleDto>().ReverseMap();

            CreateMap<ResidentFrequentlyGuest, AddResidentFrequentGuestsDto>().ReverseMap();
            CreateMap<AddResidentFrequentGuests, ResidentFrequentlyGuest>().ReverseMap();
            CreateMap<ResidentFrequentlyGuest, UpdateResidentFrequentGuestsDto>().ReverseMap();
            CreateMap<UpdateResidentFrequentlyGuests, ResidentFrequentlyGuest>().ReverseMap();
            CreateMap<ResidentFrequentlyGuest, ResidentFrequentGuestsDto>().ReverseMap();
        }
    }
}
