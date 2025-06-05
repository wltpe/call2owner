using Microsoft.EntityFrameworkCore;
using System.Data;
using Call2Owner.Models;
using Call2Owner.Models;
using Call2Owner.DTO;
using Utilities;
using System.Reflection;

namespace Oversight
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<Modules> Modules { get; set; } = null!;
        public virtual DbSet<ModulePermission> ModulePermissions { get; set; } = null!;
        public virtual DbSet<Permissions> Permissions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleClaim> RoleClaims { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserParent> UserParents { get; set; } = null!;

        public virtual DbSet<Society> Society { get; set; } = null!;
        public virtual DbSet<SocietyDocumentRequiredToRegister> SocietyDocumentRequiredToRegister { get; set; } = null!;
        public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; } = null!;
        public virtual DbSet<SocietyBuilding> SocietyBuilding { get; set; } = null!;
        public virtual DbSet<SocietyFlat> SocietyFlat { get; set; } = null!;
        public virtual DbSet<Country> Country { get; set; } = null!;
        public virtual DbSet<State> State { get; set; } = null!;
        public virtual DbSet<City> City { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
