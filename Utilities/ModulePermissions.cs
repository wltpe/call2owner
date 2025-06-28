using System.Security.Claims;

namespace Utilities
{
    public class ModulePermissionData
    {
        public int ModuleId { get; set; }
        public List<PermissionData> Permissions { get; set; } = new();
    }

    public class PermissionData
    {
        public int PermissionId { get; set; }
    }

    public class UserRoles
    {
        public const string SuperAdmin = "301";
        public const string Admin = "302";
        public const string SocietyAdmin = "303";
        public const string Resident = "304";
        public const string Guest = "305";
        public const string SocietyUser = "306";
        public const string Maid = "307";
        public const string Milkman = "308";
        public const string CarCleaner = "309";
        public const string TuitionTeacher = "310";
        public const string Nurse = "311";
        public const string Staff = "312";
        public const string Others = "313";
        public const string DogWalker = "314";
    }

    public class Module
    {
        public const string Society = "101";
        public const string SocietyBuilding = "102";
        public const string SocietyDocument = "103";
        public const string SocietyDocumentUploaded = "104";
        public const string SocietyFlats = "105";
        public const string VehicleTag = "106";
        public const string VehicleQRScan = "107";
        public const string UserManagement = "108";
        public const string Resident = "109";
    }

    public class Permission
    {
        public const string Add = "201";
        public const string Update = "202";
        public const string Delete = "203";
        public const string GetById = "204";
        public const string GetAll = "205";
        public const string VerifyDocument = "206";
        public const string VerifySociety = "207";
        public const string ResendVerificationEmail = "208";
        public const string ForgetEmailPwd = "209";
        public const string AddAdminUser = "210";
        public const string AddUser = "211";
    }

    public class EntityType 
    {
        public const string ResidentType = "3";
        public const string PetType = "9";
    }

    public class EntityTypeDetail
    {
        public const string Family = "11";
        public const string Pets = "12";
        public const string Dog = "19";
        public const string Cat = "20";
        public const string Bird = "21";
        public const string Rabbit = "22";
        public const string Hamsters = "23";
        public const string GuineaPig = "24";
    }
}