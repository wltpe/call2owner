using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Call2Owner.API.Model;

public partial class Call2ownerDevContext : DbContext
{
    public Call2ownerDevContext()
    {
    }

    public Call2ownerDevContext(DbContextOptions<Call2ownerDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModulePermission> ModulePermissions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<ResidentDocumentRequiredToRegister> ResidentDocumentRequiredToRegisters { get; set; }

    public virtual DbSet<ResidentDocumentUploaded> ResidentDocumentUploadeds { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Society> Societies { get; set; }

    public virtual DbSet<SocietyBuilding> SocietyBuildings { get; set; }

    public virtual DbSet<SocietyDocumentRequiredToRegister> SocietyDocumentRequiredToRegisters { get; set; }

    public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploadeds { get; set; }

    public virtual DbSet<SocietyFlat> SocietyFlats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserParent> UserParents { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=103.67.182.216,2499;Initial Catalog=call2owner_dev;User ID=sa;Password=j3x0rGPI1ozN;Trust Server Certificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.HasIndex(e => e.ModuleId, "IX_ModulePermissions_ModuleId");

            entity.HasOne(d => d.Module).WithMany(p => p.ModulePermissions).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resident__3214EC07B794F00A");

            entity.ToTable("Resident");

            entity.Property(e => e.ApprovedBy).HasMaxLength(255);
            entity.Property(e => e.ApprovedComment).HasMaxLength(1000);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<ResidentDocumentRequiredToRegister>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resident__3214EC07F1E15B88");

            entity.ToTable("ResidentDocumentRequiredToRegister");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(255);
        });

        modelBuilder.Entity<ResidentDocumentUploaded>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Resident__3214EC07E9942624");

            entity.ToTable("ResidentDocumentUploaded");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.ParentRoleId, "IX_Roles_ParentRoleId");

            entity.HasOne(d => d.ParentRole).WithMany(p => p.InverseParentRole).HasForeignKey(d => d.ParentRoleId);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Society>(entity =>
        {
            entity.ToTable("Society");
        });

        modelBuilder.Entity<SocietyBuilding>(entity =>
        {
            entity.ToTable("SocietyBuilding");
        });

        modelBuilder.Entity<SocietyDocumentRequiredToRegister>(entity =>
        {
            entity.ToTable("SocietyDocumentRequiredToRegister");
        });

        modelBuilder.Entity<SocietyDocumentUploaded>(entity =>
        {
            entity.ToTable("SocietyDocumentUploaded");
        });

        modelBuilder.Entity<SocietyFlat>(entity =>
        {
            entity.ToTable("SocietyFlat");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");

            entity.Property(e => e.Otp)
                .HasMaxLength(10)
                .HasColumnName("OTP");
            entity.Property(e => e.RefreshToken).HasMaxLength(1000);

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<UserParent>(entity =>
        {
            entity.ToTable("UserParent");

            entity.HasIndex(e => e.UserId, "IX_UserParent_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserParents).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProf__3214EC078F61538E");

            entity.ToTable("UserProfile");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
