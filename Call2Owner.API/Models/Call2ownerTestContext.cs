using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Call2Owner.Models;

public partial class Call2ownerTestContext : DbContext
{
    public Call2ownerTestContext()
    {
    }

    public Call2ownerTestContext(DbContextOptions<Call2ownerTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CabCompany> CabCompany { get; set; }

    public virtual DbSet<City> City { get; set; }

    public virtual DbSet<Country> Country { get; set; }

    public virtual DbSet<DeliveryCompany> DeliveryCompany { get; set; }

    public virtual DbSet<EntityType> EntityType { get; set; }

    public virtual DbSet<EntityTypeDetail> EntityTypeDetail { get; set; }

    public virtual DbSet<Module> Module { get; set; }

    public virtual DbSet<ModulePermission> ModulePermission { get; set; }

    public virtual DbSet<Permission> Permission { get; set; }

    public virtual DbSet<Resident> Resident { get; set; }

    public virtual DbSet<ResidentDocumentUploaded> ResidentDocumentUploaded { get; set; }

    public virtual DbSet<ResidentFamily> ResidentFamily { get; set; }

    public virtual DbSet<ResidentFrequentlyEntry> ResidentFrequentlyEntry { get; set; }

    public virtual DbSet<ResidentFrequentlyGuest> ResidentFrequentlyGuest { get; set; }

    public virtual DbSet<ResidentPet> ResidentPet { get; set; }

    public virtual DbSet<ResidentVehicle> ResidentVehicle { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<RoleClaim> RoleClaim { get; set; }

    public virtual DbSet<Society> Society { get; set; }

    public virtual DbSet<SocietyBuilding> SocietyBuilding { get; set; }

    public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; }

    public virtual DbSet<SocietyFlat> SocietyFlat { get; set; }

    public virtual DbSet<SocietyUser> SocietyUser { get; set; }

    public virtual DbSet<SocietyUserDocumentUploaded> SocietyUserDocumentUploaded { get; set; }

    public virtual DbSet<State> State { get; set; }

    public virtual DbSet<User> User { get; set; }

    public virtual DbSet<UserParent> UserParent { get; set; }

    public virtual DbSet<UserProfile> UserProfile { get; set; }

    public virtual DbSet<VisitingHelpCategory> VisitingHelpCategory { get; set; }

    public virtual DbSet<VisitingHelpCategoryCompany> VisitingHelpCategoryCompany { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=103.67.182.216,2499;Initial Catalog=call2owner_test;User ID=sa;Password=j3x0rGPI1ozN;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CabCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CabCompa__3214EC07CF373046");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Logo).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasIndex(e => e.StateId, "IX_City_StateId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.State).WithMany(p => p.City)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DeliveryCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07796F7DC6");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Logo).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EntityType>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DafaultValue).HasMaxLength(200);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Label).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EntityTypeDetail>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeId, "IX_EntityTypeDetail_EntityTypeId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(200);

            entity.HasOne(d => d.EntityType).WithMany(p => p.EntityTypeDetail)
                .HasForeignKey(d => d.EntityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntityTypeDetails_EntityType");
        });

        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.HasIndex(e => e.ModuleId, "IX_ModulePermissions_ModuleId");

            entity.HasOne(d => d.Module).WithMany(p => p.ModulePermission).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeDetailId, "IX_Resident_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyFlatId, "IX_Resident_SocietyFlatId");

            entity.HasIndex(e => e.UserId, "IX_Resident_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApprovedBy).HasMaxLength(255);
            entity.Property(e => e.ApprovedComment).HasMaxLength(1000);
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.Resident)
                .HasForeignKey(d => d.EntityTypeDetailId)
                .HasConstraintName("FK_Resident_EntityTypeDetails");

            entity.HasOne(d => d.SocietyFlat).WithMany(p => p.Resident)
                .HasForeignKey(d => d.SocietyFlatId)
                .HasConstraintName("FK_Resident_SocietyFlat");

            entity.HasOne(d => d.User).WithMany(p => p.Resident)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resident_Users");
        });

        modelBuilder.Entity<ResidentDocumentUploaded>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeDetailId, "IX_ResidentDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.ResidentId, "IX_ResidentDocumentUploaded_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.ResidentDocumentUploaded).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentDocumentUploaded).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<ResidentFamily>(entity =>
        {
            entity.HasIndex(e => e.ResidentId, "IX_ResidentFamily_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.ExitType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FamilyType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFamily)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFamily_Resident");
        });

        modelBuilder.Entity<ResidentFrequentlyEntry>(entity =>
        {
            entity.HasIndex(e => e.CabCompanyId, "IX_ResidentFrequentlyEntry_CabCompanyId");

            entity.HasIndex(e => e.DeliveryCompanyId, "IX_ResidentFrequentlyEntry_DeliveryCompanyId");

            entity.HasIndex(e => e.ResidentId, "IX_ResidentFrequentlyEntry_ResidentId");

            entity.HasIndex(e => e.VisitingHelpCategoryCompanyId, "IX_ResidentFrequentlyEntry_VisitingHelpCategoryCompanyId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AllowEntryInNext)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DaysOfWeek)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.EntriesPerDay)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntryType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FrequentlyType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UniqueEntryCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Validity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VehicleNo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.CabCompany).WithMany(p => p.ResidentFrequentlyEntry)
                .HasForeignKey(d => d.CabCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_CabCompany");

            entity.HasOne(d => d.DeliveryCompany).WithMany(p => p.ResidentFrequentlyEntry)
                .HasForeignKey(d => d.DeliveryCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_DeliveryCompany");

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFrequentlyEntry)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFrequentlyEntry_Resident");

            entity.HasOne(d => d.VisitingHelpCategoryCompany).WithMany(p => p.ResidentFrequentlyEntry)
                .HasForeignKey(d => d.VisitingHelpCategoryCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_VisitingHelpCategoryCompany");
        });

        modelBuilder.Entity<ResidentFrequentlyGuest>(entity =>
        {
            entity.HasIndex(e => e.ResidentId, "IX_ResidentFrequentlyGuest_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AllowEntryForNext)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UniqueEntryNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.ValidFor)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFrequentlyGuest)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFrequentlyGuests_Resident");
        });

        modelBuilder.Entity<ResidentPet>(entity =>
        {
            entity.HasIndex(e => e.ResidentId, "IX_ResidentPet_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.PetBreed)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PetName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PetPicture)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PetType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.VaccinationDoc)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VaccinationType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentPet)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentPet_Resident");
        });

        modelBuilder.Entity<ResidentVehicle>(entity =>
        {
            entity.HasIndex(e => e.ResidentId, "IX_ResidentVehicle_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.FuelType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RfidTagNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("RFIdTagNumber");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.VehicleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VehicleNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VehicleType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentVehicle)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentVehicle_Resident");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.ParentRoleId, "IX_Roles_ParentRoleId");

            entity.HasOne(d => d.ParentRole).WithMany(p => p.InverseParentRole).HasForeignKey(d => d.ParentRoleId);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaim).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Society>(entity =>
        {
            entity.HasIndex(e => e.CityId, "IX_Society_CityId");

            entity.HasIndex(e => e.CountryId, "IX_Society_CountryId");

            entity.HasIndex(e => e.StateId, "IX_Society_StateId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Society)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_City");

            entity.HasOne(d => d.Country).WithMany(p => p.Society)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_Country");

            entity.HasOne(d => d.State).WithMany(p => p.Society)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_State");
        });

        modelBuilder.Entity<SocietyBuilding>(entity =>
        {
            entity.HasIndex(e => e.SocietyId, "IX_SocietyBuilding_SocietyId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyBuilding)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyBuilding_Society");
        });

        modelBuilder.Entity<SocietyDocumentUploaded>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyDocumentUploaded_SocietyId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyDocumentUploaded).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyDocumentUploaded).HasForeignKey(d => d.SocietyId);
        });

        modelBuilder.Entity<SocietyFlat>(entity =>
        {
            entity.HasIndex(e => e.SocietyBuildingId, "IX_SocietyFlat_SocietyBuildingId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyFlat_SocietyId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.SocietyBuilding).WithMany(p => p.SocietyFlat)
                .HasForeignKey(d => d.SocietyBuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyFlat_SocietyBuilding");

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyFlat)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyFlat_Society");
        });

        modelBuilder.Entity<SocietyUser>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyUser_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyUser_SocietyId");

            entity.HasIndex(e => e.UserId, "IX_SocietyUser_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyUser).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyUser).HasForeignKey(d => d.SocietyId);

            entity.HasOne(d => d.User).WithMany(p => p.SocietyUser).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SocietyUserDocumentUploaded>(entity =>
        {
            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyUserDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyUserId, "IX_SocietyUserDocumentUploaded_SocietyUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyUserDocumentUploaded).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.SocietyUser).WithMany(p => p.SocietyUserDocumentUploaded).HasForeignKey(d => d.SocietyUserId);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasIndex(e => e.CountryId, "IX_State_CountryId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.State)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Otp)
                .HasMaxLength(10)
                .HasColumnName("OTP");
            entity.Property(e => e.OtpExpireTime).HasColumnType("datetime");
            entity.Property(e => e.OtpValidatedOn).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken).HasMaxLength(1000);
            entity.Property(e => e.RefreshTokenExpireTime).HasColumnType("datetime");
            entity.Property(e => e.ResendOtpTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.VerificationCodeGenerationTime).HasColumnType("datetime");
            entity.Property(e => e.VerificationCodeValidationTime).HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.User)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserParent>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserParent_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserParent)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserParent_Users");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserProfile_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfile)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_Users");
        });

        modelBuilder.Entity<VisitingHelpCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Visiting__3214EC078E004FA9");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<VisitingHelpCategoryCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Visiting__3214EC07CE8F7A25");

            entity.HasIndex(e => e.VisitingHelpCategoryId, "IX_VisitingHelpCategoryCompany_VisitingHelpCategoryId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.VisitingHelpCategory).WithMany(p => p.VisitingHelpCategoryCompany)
                .HasForeignKey(d => d.VisitingHelpCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitingHelpCategoryCompany_VisitingHelpCategory");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
