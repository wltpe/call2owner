using Call2Owner.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Call2Owner.API
{
    public static class DbSeeder
    {
        public static void SeedIfNotExists(DataContext context)
        {
            if (!context.Module.Any())
            {
                using var transaction = context.Database.BeginTransaction();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Module ON");

                context.Module.AddRange(
                    new Module { ModuleId = 101, ModuleName = "Society" },
                    new Module { ModuleId = 102, ModuleName = "SocietyBuilding" },
                    new Module { ModuleId = 103, ModuleName = "SocietyDocument" },
                    new Module { ModuleId = 104, ModuleName = "SocietyDocumentUploaded" },
                    new Module { ModuleId = 105, ModuleName = "SocietyFlats" },
                    new Module { ModuleId = 106, ModuleName = "VehicleTag" },
                    new Module { ModuleId = 107, ModuleName = "VehicleQRScan" },
                    new Module { ModuleId = 108, ModuleName = "UserManagement" },
                    new Module { ModuleId = 109, ModuleName = "Resident" }
                );

                context.SaveChanges();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Module OFF");
                transaction.Commit();
            }

            if (!context.Permission.Any())
            {
                using var transaction = context.Database.BeginTransaction();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Permission ON");

                context.Permission.AddRange(
                    new Permission { Id = 201, PermissionName = "Add" },
                    new Permission { Id = 202, PermissionName = "Update" },
                    new Permission { Id = 203, PermissionName = "Delete" },
                    new Permission { Id = 204, PermissionName = "GetById" },
                    new Permission { Id = 205, PermissionName = "GetAll" },
                    new Permission { Id = 206, PermissionName = "VerifyDocument" },
                    new Permission { Id = 207, PermissionName = "VerifySociety" },
                    new Permission { Id = 208, PermissionName = "ResendVerificationEmail" },
                    new Permission { Id = 209, PermissionName = "ForgetEmailPwd" }
                );

                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Permission OFF");
                transaction.Commit();
            }

            if (!context.ModulePermission.Any())
            {
                using var transaction = context.Database.BeginTransaction();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ModulePermission ON");

                context.ModulePermission.AddRange(
                    new[]
                    {
                            new ModulePermission
                            {
                                Id = 1,
                                ModuleId = 101,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 3,
                                ModuleId = 102,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 4,
                                ModuleId = 103,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 5,
                                ModuleId = 104,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 6,
                                ModuleId = 105,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 7,
                                ModuleId = 106,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 8,
                                ModuleId = 107,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 9,
                                ModuleId = 108,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            },
                            new ModulePermission
                            {
                                Id = 10,
                                ModuleId = 109,
                                PermissionsJson = "[{\"PermissionId\":201},{\"PermissionId\":202},{\"PermissionId\":203},{\"PermissionId\":204},{\"PermissionId\":205},{\"PermissionId\":206},{\"PermissionId\":207},{\"PermissionId\":208},{\"PermissionId\":209}]"
                            }
                    }
                );

                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.ModulePermission OFF");
                transaction.Commit();
            }


            if (!context.Role.Any())
            {
                using var transaction = context.Database.BeginTransaction();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Role ON");

                context.Role.AddRange(
                    new Role { Id = 301, RoleName = "SuperAdmin", DisplayName = "Super Admin", ParentRoleId = null },
                    new Role { Id = 302, RoleName = "Admin", DisplayName = "Admin", ParentRoleId = 301 },
                    new Role { Id = 303, RoleName = "SocietyAdmin", DisplayName = "Society Admin", ParentRoleId = 302 },
                    new Role { Id = 304, RoleName = "Resident", DisplayName = "Resident", ParentRoleId = 303 },
                    new Role { Id = 305, RoleName = "Guest", DisplayName = "Guest", ParentRoleId = 304 }
                );

                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Role OFF");
                transaction.Commit();
            }

            if (!context.RoleClaim.Any())
            {
                using var transaction = context.Database.BeginTransaction();

                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.RoleClaim ON");

                context.RoleClaim.AddRange(
                    new RoleClaim
                    {
                        Id = 1,
                        RoleId = 301,
                        ModulePermissionsJson = @"[
                        {""ModuleId"":101,""Permissions"":{""PermissionId"":201},""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205},""PermissionId"":206},{""PermissionId"":207},""PermissionId"":208},{""PermissionId"":209}]},
                        {""ModuleId"":102,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":103,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":104,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":105,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":106,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":107,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":108,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":109,""Permissions"":[{""PermissionId"":201},{""PermissionId"":202},""PermissionId"":203},{""PermissionId"":204},{""PermissionId"":205}]}
                    ]"
                    },
                    new RoleClaim
                    {
                        Id = 2,
                        RoleId = 302,
                        ModulePermissionsJson = @"[
                        {""ModuleId"":101,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":102,""Permissions"":[{""PermissionId"":201},""PermissionId"":202},""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":103,""Permissions"":[{""PermissionId"":204},""PermissionId"":205}]},                        {""ModuleId"":104,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":105,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":106,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":107,""Permissions"":[{""PermissionId"":204},""PermissionId"":205}]},                        {""ModuleId"":108,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},                        {""ModuleId"":109,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]}
                    ]"
                    },
                    new RoleClaim
                    {
                        Id = 3,
                        RoleId = 303,
                        ModulePermissionsJson = @"[
                        {""ModuleId"":101,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":102,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":103,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":104,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":105,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":106,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":107,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":108,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":109,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]}
                    ]"
                    },
                    new RoleClaim
                    {
                        Id = 4,
                        RoleId = 304,
                        ModulePermissionsJson = @"[
                        {""ModuleId"":105,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":106,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":107,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]},
                        {""ModuleId"":109,""Permissions"":[{""PermissionId"":204},{""PermissionId"":205}]}
                    ]"
                    },
                    new RoleClaim
                    {
                        Id = 5,
                        RoleId = 305,
                        ModulePermissionsJson = @"[
                        {""ModuleId"":105,""Permissions"":[{""PermissionId"":204}]},
                        {""ModuleId"":109,""Permissions"":[{""PermissionId"":204}]}
                    ]"
                    }
                );

                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.RoleClaim OFF");
                transaction.Commit();
            }
        }
    }
}
