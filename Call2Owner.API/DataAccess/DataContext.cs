using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Call2Owner.Models;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CabCompany> CabCompany { get; set; }

    public virtual DbSet<City> City { get; set; }

    public virtual DbSet<Country> Country { get; set; }

    public virtual DbSet<DeliveryCompany> DeliveryCompany { get; set; }

    public virtual DbSet<EntityType> EntityType { get; set; }

    public virtual DbSet<EntityTypeDetail> EntityTypeDetails { get; set; }

    public virtual DbSet<Module> Module { get; set; }

    public virtual DbSet<ModulePermission> ModulePermissions { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Resident> Resident { get; set; }

    public virtual DbSet<ResidentDocumentRequiredToRegister> ResidentDocumentRequiredToRegister { get; set; }

    public virtual DbSet<ResidentDocumentUploaded> ResidentDocumentUploaded { get; set; }

    public virtual DbSet<ResidentFamily> ResidentFamily { get; set; }

    public virtual DbSet<ResidentFrequentlyEntry> ResidentFrequentlyEntry { get; set; }

    public virtual DbSet<ResidentFrequentlyGuest> ResidentFrequentlyGuests { get; set; }

    public virtual DbSet<ResidentPet> ResidentPet { get; set; }

    public virtual DbSet<ResidentVehicle> ResidentVehicle { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Society> Society { get; set; }

    public virtual DbSet<SocietyBuilding> SocietyBuilding { get; set; }

    public virtual DbSet<SocietyDocumentRequiredToRegister> SocietyDocumentRequiredToRegister { get; set; }

    public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; }

    public virtual DbSet<SocietyFlat> SocietyFlat { get; set; }

    public virtual DbSet<State> State { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserParent> UserParent { get; set; }

    public virtual DbSet<UserProfile> UserProfile { get; set; }

    public virtual DbSet<VisitingHelpCategory> VisitingHelpCategory { get; set; }

    public virtual DbSet<VisitingHelpCategoryCompany> VisitingHelpCategoryCompany { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CabCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CabCompa__3214EC07CF373046");

            entity.ToTable("CabCompany");

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
            entity.ToTable("City");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<DeliveryCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07796F7DC6");

            entity.ToTable("DeliveryCompany");

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
            entity.ToTable("EntityType");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DafaultValue).HasMaxLength(200);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Label).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<EntityTypeDetail>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(200);

            entity.HasOne(d => d.EntityType).WithMany(p => p.EntityTypeDetails)
                .HasForeignKey(d => d.EntityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntityTypeDetails_EntityType");
        });

        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.HasIndex(e => e.ModuleId, "IX_ModulePermissions_ModuleId");

            entity.HasOne(d => d.Module).WithMany(p => p.ModulePermissions).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.ToTable("Resident");

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

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.Residents)
                .HasForeignKey(d => d.EntityTypeDetailId)
                .HasConstraintName("FK_Resident_EntityTypeDetails");

            entity.HasOne(d => d.SocietyFlat).WithMany(p => p.Residents)
                .HasForeignKey(d => d.SocietyFlatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resident_SocietyFlat");

            entity.HasOne(d => d.User).WithMany(p => p.Residents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resident_Users");
        });

        modelBuilder.Entity<ResidentDocumentRequiredToRegister>(entity =>
        {
            entity.ToTable("ResidentDocumentRequiredToRegister");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.ResidentDocumentRequiredToRegisters)
                .HasForeignKey(d => d.EntityTypeDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentDocumentRequiredToRegister_EntityTypeDetails");
        });

        modelBuilder.Entity<ResidentDocumentUploaded>(entity =>
        {
            entity.ToTable("ResidentDocumentUploaded");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Url).HasMaxLength(500);

            entity.HasOne(d => d.ResidentDocumentRequiredToRegister).WithMany(p => p.ResidentDocumentUploadeds)
                .HasForeignKey(d => d.ResidentDocumentRequiredToRegisterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentDocumentUploaded_DocRequirement");

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentDocumentUploadeds)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentDocumentUploaded_Resident");
        });

        modelBuilder.Entity<ResidentFamily>(entity =>
        {
            entity.ToTable("ResidentFamily");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFamilies)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFamily_Resident");
        });

        modelBuilder.Entity<ResidentFrequentlyEntry>(entity =>
        {
            entity.ToTable("ResidentFrequentlyEntry");

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

            entity.HasOne(d => d.CabCompany).WithMany(p => p.ResidentFrequentlyEntries)
                .HasForeignKey(d => d.CabCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_CabCompany");

            entity.HasOne(d => d.DeliveryCompany).WithMany(p => p.ResidentFrequentlyEntries)
                .HasForeignKey(d => d.DeliveryCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_DeliveryCompany");

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFrequentlyEntries)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFrequentlyEntry_Resident");

            entity.HasOne(d => d.VisitingHelpCategoryCompany).WithMany(p => p.ResidentFrequentlyEntries)
                .HasForeignKey(d => d.VisitingHelpCategoryCompanyId)
                .HasConstraintName("FK_ResidentFrequentlyEntry_VisitingHelpCategoryCompany");
        });

        modelBuilder.Entity<ResidentFrequentlyGuest>(entity =>
        {
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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFrequentlyGuests)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFrequentlyGuests_Resident");
        });

        modelBuilder.Entity<ResidentPet>(entity =>
        {
            entity.ToTable("ResidentPet");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentPets)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentPet_Resident");
        });

        modelBuilder.Entity<ResidentVehicle>(entity =>
        {
            entity.ToTable("ResidentVehicle");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentVehicles)
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

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Society>(entity =>
        {
            entity.ToTable("Society");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Societies)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_City");

            entity.HasOne(d => d.Country).WithMany(p => p.Societies)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_Country");

            entity.HasOne(d => d.State).WithMany(p => p.Societies)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Society_State");
        });

        modelBuilder.Entity<SocietyBuilding>(entity =>
        {
            entity.ToTable("SocietyBuilding");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyBuildings)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyBuilding_Society");
        });

        modelBuilder.Entity<SocietyDocumentRequiredToRegister>(entity =>
        {
            entity.ToTable("SocietyDocumentRequiredToRegister");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyDocumentRequiredToRegisters)
                .HasForeignKey(d => d.EntityTypeDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyDocRequired_EntityTypeDetails");
        });

        modelBuilder.Entity<SocietyDocumentUploaded>(entity =>
        {
            entity.ToTable("SocietyDocumentUploaded");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.SocietyDocumentRequiredToRegister).WithMany(p => p.SocietyDocumentUploadeds)
                .HasForeignKey(d => d.SocietyDocumentRequiredToRegisterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyDocumentUploaded_RequiredDoc");

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyDocumentUploadeds)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyDocumentUploaded_Society");
        });

        modelBuilder.Entity<SocietyFlat>(entity =>
        {
            entity.ToTable("SocietyFlat");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.SocietyBuilding).WithMany(p => p.SocietyFlats)
                .HasForeignKey(d => d.SocietyBuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyFlat_SocietyBuilding");

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyFlats)
                .HasForeignKey(d => d.SocietyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyFlat_Society");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
        });

        modelBuilder.Entity<User>(entity =>
        {
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

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserParent>(entity =>
        {
            entity.ToTable("UserParent");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserParents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserParent_Users");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("UserProfile");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProfile_Users");
        });

        modelBuilder.Entity<VisitingHelpCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Visiting__3214EC078E004FA9");

            entity.ToTable("VisitingHelpCategory");

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

            entity.ToTable("VisitingHelpCategoryCompany");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.VisitingHelpCategory).WithMany(p => p.VisitingHelpCategoryCompanies)
                .HasForeignKey(d => d.VisitingHelpCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitingHelpCategoryCompany_VisitingHelpCategory");
        });
    }
}
