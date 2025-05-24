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

    }

    public class Module
    {
        public const string Society = "1";
        public const string VehicleTag = "2";
        public const string VehicleQRScan = "3";
        public const string UserManagement = "4";
    }

    public class Permission
    {
        public const string Add = "1";
        public const string Update = "2";
        public const string Delete = "3";
        public const string GetById = "4";
        public const string GetAll = "5";
        public const string VerifyDocument = "6";
        public const string VerifySociety = "7";
        public const string ResendVerificationEmail = "10";
        public const string ForgetEmailPwd = "11";
    }
}