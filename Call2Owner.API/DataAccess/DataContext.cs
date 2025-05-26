using Microsoft.EntityFrameworkCore;
using System.Data;
using Oversight.Model;
using Oversight.Models;
using Oversight.DTO;

namespace Oversight
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<ModulePermission> ModulePermissions { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleClaim> RoleClaims { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserParent> UserParents { get; set; } = null!;

        public virtual DbSet<Society> Society { get; set; } = null!;
        public virtual DbSet<SocietyDocumentRequiredToRegister> SocietyDocumentRequiredToRegister { get; set; } = null!;
        public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; } = null!;
        public virtual DbSet<SocietyFlat> SocietyFlat { get; set; } = null!;
        public virtual DbSet<SocietyBuilding> SocietyBuilding { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
