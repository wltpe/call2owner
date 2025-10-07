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

    public virtual DbSet<AdminWallet> AdminWallet { get; set; }
    public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
    public virtual DbSet<AppConfig> AppConfig { get; set; }
    public virtual DbSet<Beneficiary> Beneficiary { get; set; }
    public virtual DbSet<CabCompany> CabCompany { get; set; }
    public virtual DbSet<CircleCode> CircleCode { get; set; }
    public virtual DbSet<CircleMaster> CircleMaster { get; set; }
    public virtual DbSet<City> City { get; set; }
    public virtual DbSet<Config> Config { get; set; }
    public virtual DbSet<Counter> Counter { get; set; }
    public virtual DbSet<Country> Country { get; set; }
    public virtual DbSet<CustomerWallet> CustomerWallet { get; set; }
    public virtual DbSet<DeliveryCompany> DeliveryCompany { get; set; }
    public virtual DbSet<EntityType> EntityType { get; set; }
    public virtual DbSet<EntityTypeDetail> EntityTypeDetail { get; set; }
    public virtual DbSet<ErrorCode> ErrorCode { get; set; }
    public virtual DbSet<GatewayTransaction> GatewayTransaction { get; set; }
    public virtual DbSet<Hash> Hash { get; set; }
    public virtual DbSet<Job> Job { get; set; }
    public virtual DbSet<JobParameter> JobParameter { get; set; }
    public virtual DbSet<JobQueue> JobQueue { get; set; }
    public virtual DbSet<List> List { get; set; }
    public virtual DbSet<LogMessage> LogMessage { get; set; }
    public virtual DbSet<Module> Module { get; set; }
    public virtual DbSet<ModulePermission> ModulePermission { get; set; }
    public virtual DbSet<OperatorCode> OperatorCode { get; set; }
    public virtual DbSet<OperatorMaster> OperatorMaster { get; set; }
    public virtual DbSet<OperatorMasterServiceType> OperatorMasterServiceType { get; set; }
    public virtual DbSet<Permission> Permission { get; set; }
    public virtual DbSet<PlanCircleCode> PlanCircleCode { get; set; }
    public virtual DbSet<PlanOperatorCode> PlanOperatorCode { get; set; }
    public virtual DbSet<PlanProvider> PlanProvider { get; set; }
    public virtual DbSet<Provider> Provider { get; set; }
    public virtual DbSet<Queue> Queue { get; set; }
    public virtual DbSet<RechargeRequest> RechargeRequest { get; set; }
    public virtual DbSet<RechargeRequestDispute> RechargeRequestDispute { get; set; }
    public virtual DbSet<Refund> Refund { get; set; }
    public virtual DbSet<RequestMapper> RequestMapper { get; set; }
    public virtual DbSet<Resident> Resident { get; set; }
    public virtual DbSet<ResidentDocumentUploaded> ResidentDocumentUploaded { get; set; }
    public virtual DbSet<ResidentFamily> ResidentFamily { get; set; }
    public virtual DbSet<ResidentFrequentlyEntry> ResidentFrequentlyEntry { get; set; }
    public virtual DbSet<ResidentFrequentlyGuest> ResidentFrequentlyGuest { get; set; }
    public virtual DbSet<ResidentPet> ResidentPet { get; set; }
    public virtual DbSet<ResidentVehicle> ResidentVehicle { get; set; }
    public virtual DbSet<Resource> Resource { get; set; }
    public virtual DbSet<ResourceMapper> ResourceMapper { get; set; }
    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<RoleClaim> RoleClaim { get; set; }
    public virtual DbSet<Schema> Schema { get; set; }
    public virtual DbSet<Server> Server { get; set; }
    public virtual DbSet<ServiceCategory> ServiceCategory { get; set; }
    public virtual DbSet<ServiceType> ServiceType { get; set; }
    public virtual DbSet<Set> Set { get; set; }
    public virtual DbSet<Setting> Setting { get; set; }
    public virtual DbSet<Society> Society { get; set; }
    public virtual DbSet<SocietyBuilding> SocietyBuilding { get; set; }
    public virtual DbSet<SocietyDocumentUploaded> SocietyDocumentUploaded { get; set; }
    public virtual DbSet<SocietyFlat> SocietyFlat { get; set; }
    public virtual DbSet<SocietyUser> SocietyUser { get; set; }
    public virtual DbSet<SocietyUserDocumentUploaded> SocietyUserDocumentUploaded { get; set; }
    public virtual DbSet<State> State { get; set; }
    public virtual DbSet<State1> State1 { get; set; }
    public virtual DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
    public virtual DbSet<SystemInfo> SystemInfo { get; set; }
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<UserFavorite> UserFavorite { get; set; }
    public virtual DbSet<UserParent> UserParent { get; set; }
    public virtual DbSet<UserProfile> UserProfile { get; set; }
    public virtual DbSet<UserRedeemCode> UserRedeemCode { get; set; }
    public virtual DbSet<UserReferralCode> UserReferralCode { get; set; }
    public virtual DbSet<UserSubscriptionPlan> UserSubscriptionPlan { get; set; }
    public virtual DbSet<VisitingHelpCategory> VisitingHelpCategory { get; set; }
    public virtual DbSet<VisitingHelpCategoryCompany> VisitingHelpCategoryCompany { get; set; }

    public virtual DbSet<SocietyBuildingType> SocietyBuildingType { get; set; }
    public virtual DbSet<SocietyUserFlatWorkingHistory> SocietyUserFlatWorkingHistory { get; set; }

    public virtual DbSet<SocietyUserProfile> SocietyUserProfile { get; set; }

    public virtual DbSet<SocietyUserTimeSlot> SocietyUserTimeSlot { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminWallet>(entity =>
        {
            entity.ToTable("AdminWallet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AbsoluteMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AdminMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomerDiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentGatewayAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMode).HasDefaultValue("");
            entity.Property(e => e.RechargeAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Provider).WithMany(p => p.AdminWallets).HasForeignKey(d => d.ProviderId);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.AdminWallets).HasForeignKey(d => d.UserName);
        });

        modelBuilder.Entity<AggregatedCounter>(entity =>
        {
            entity.HasKey(e => e.Key).HasName("PK_HangFire_CounterAggregated");

            entity.ToTable("AggregatedCounter", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<AppConfig>(entity =>
        {
            entity.ToTable("AppConfig");

            entity.Property(e => e.ConfigName).HasMaxLength(200);
            entity.Property(e => e.ConfigType).HasMaxLength(200);
            entity.Property(e => e.PropertyKey).HasMaxLength(200);
            entity.Property(e => e.PropertyLabel).HasMaxLength(200);
        });

        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.ToTable("Beneficiary");

            entity.HasIndex(e => e.UserName, "IX_Beneficiary_UserName");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.Beneficiaries).HasForeignKey(d => d.UserName);
        });

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

        modelBuilder.Entity<CircleCode>(entity =>
        {
            entity.Property(e => e.Circle).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);

            entity.HasOne(d => d.MapCircle1Navigation).WithMany(p => p.CircleCodeMapCircle1Navigations).HasForeignKey(d => d.MapCircle1);

            entity.HasOne(d => d.MapCircle2Navigation).WithMany(p => p.CircleCodeMapCircle2Navigations).HasForeignKey(d => d.MapCircle2);

            entity.HasOne(d => d.MapCircle3Navigation).WithMany(p => p.CircleCodeMapCircle3Navigations).HasForeignKey(d => d.MapCircle3);

            entity.HasOne(d => d.Provider).WithMany(p => p.CircleCodes)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CircleMaster>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.HasIndex(e => e.StateId, "IX_City_StateId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<Config>(entity =>
        {
            entity.Property(e => e.ConfigKey).HasMaxLength(200);
            entity.Property(e => e.ConfigValue).HasMaxLength(200);
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_Counter");

            entity.ToTable("Counter", "HangFire");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CustomerWallet>(entity =>
        {
            entity.ToTable("CustomerWallet");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Credit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Debit).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.GatewayTransaction).WithMany(p => p.CustomerWallets).HasForeignKey(d => d.GatewayTransactionId);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.CustomerWallets).HasForeignKey(d => d.UserName);
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
            entity.ToTable("EntityTypeDetail");

            entity.HasIndex(e => e.EntityTypeId, "IX_EntityTypeDetail_EntityTypeId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(200);

            entity.HasOne(d => d.EntityType).WithMany(p => p.EntityTypeDetails)
                .HasForeignKey(d => d.EntityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EntityTypeDetails_EntityType");
        });

        modelBuilder.Entity<ErrorCode>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ErrorDescription).HasMaxLength(255);

            entity.HasOne(d => d.Provider).WithMany(p => p.ErrorCodes)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GatewayTransaction>(entity =>
        {
            entity.ToTable("GatewayTransaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.GatewayTransactions).HasForeignKey(d => d.UserName);
        });

        modelBuilder.Entity<Hash>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Field }).HasName("PK_HangFire_Hash");

            entity.ToTable("Hash", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Field).HasMaxLength(100);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Job");

            entity.ToTable("Job", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName").HasFilter("([StateName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(20);
        });

        modelBuilder.Entity<JobParameter>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Name }).HasName("PK_HangFire_JobParameter");

            entity.ToTable("JobParameter", "HangFire");

            entity.Property(e => e.Name).HasMaxLength(40);

            entity.HasOne(d => d.Job).WithMany(p => p.JobParameters)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_JobParameter_Job");
        });

        modelBuilder.Entity<JobQueue>(entity =>
        {
            entity.HasKey(e => new { e.Queue, e.Id }).HasName("PK_HangFire_JobQueue");

            entity.ToTable("JobQueue", "HangFire");

            entity.Property(e => e.Queue).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FetchedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Id }).HasName("PK_HangFire_List");

            entity.ToTable("List", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LogMessage>(entity =>
        {
            entity.ToTable("LogMessage");

            entity.Property(e => e.Ip).HasColumnName("IP");
            entity.Property(e => e.RequestUrl).HasMaxLength(100);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Module");
        });

        modelBuilder.Entity<ModulePermission>(entity =>
        {
            entity.ToTable("ModulePermission");

            entity.HasIndex(e => e.ModuleId, "IX_ModulePermissions_ModuleId");

            entity.HasOne(d => d.Module).WithMany(p => p.ModulePermissions).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<OperatorCode>(entity =>
        {
            entity.Property(e => e.AdminMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OperatorName).HasMaxLength(200);

            entity.HasOne(d => d.MapOperator1Navigation).WithMany(p => p.OperatorCodeMapOperator1Navigations).HasForeignKey(d => d.MapOperator1);

            entity.HasOne(d => d.MapOperator2Navigation).WithMany(p => p.OperatorCodeMapOperator2Navigations).HasForeignKey(d => d.MapOperator2);

            entity.HasOne(d => d.MapOperator3Navigation).WithMany(p => p.OperatorCodeMapOperator3Navigations).HasForeignKey(d => d.MapOperator3);

            entity.HasOne(d => d.MapOperator4Navigation).WithMany(p => p.OperatorCodeMapOperator4Navigations).HasForeignKey(d => d.MapOperator4);

            entity.HasOne(d => d.MapOperator5Navigation).WithMany(p => p.OperatorCodeMapOperator5Navigations).HasForeignKey(d => d.MapOperator5);

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.OperatorCodes)
                .HasForeignKey(d => d.OperatorMasterServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Provider).WithMany(p => p.OperatorCodes)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OperatorMaster>(entity =>
        {
            entity.Property(e => e.MobileImage).HasMaxLength(1000);
            entity.Property(e => e.OperatorMasterCode).HasMaxLength(20);
            entity.Property(e => e.OperatorName).HasMaxLength(100);
            entity.Property(e => e.OptionalImage).HasMaxLength(1000);
            entity.Property(e => e.WebImage).HasMaxLength(1000);
        });

        modelBuilder.Entity<OperatorMasterServiceType>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.MobileImage).HasMaxLength(1000);
            entity.Property(e => e.OperatorName).HasMaxLength(100);
            entity.Property(e => e.OptionalImage).HasMaxLength(1000);
            entity.Property(e => e.WebImage).HasMaxLength(1000);

            entity.HasOne(d => d.OperatorMaster).WithMany(p => p.OperatorMasterServiceTypes)
                .HasForeignKey(d => d.OperatorMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ServiceType).WithMany(p => p.OperatorMasterServiceTypes)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permission");
        });

        modelBuilder.Entity<PlanCircleCode>(entity =>
        {
            entity.Property(e => e.Circle).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);

            entity.HasOne(d => d.CircleMaster).WithMany(p => p.PlanCircleCodes).HasForeignKey(d => d.CircleMasterId);

            entity.HasOne(d => d.PlanProvider).WithMany(p => p.PlanCircleCodes)
                .HasForeignKey(d => d.PlanProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PlanOperatorCode>(entity =>
        {
            entity.Property(e => e.PlanCode).HasMaxLength(50);
            entity.Property(e => e.PlanOperator).HasMaxLength(100);

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.PlanOperatorCodes)
                .HasForeignKey(d => d.OperatorMasterServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PlanProvider).WithMany(p => p.PlanOperatorCodes)
                .HasForeignKey(d => d.PlanProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PlanProvider>(entity =>
        {
            entity.Property(e => e.PlanProviderDetail).HasMaxLength(200);
            entity.Property(e => e.PlanProviderName).HasMaxLength(100);
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.Property(e => e.ProviderDetail).HasMaxLength(200);
            entity.Property(e => e.ProviderName).HasMaxLength(100);
        });

        modelBuilder.Entity<Queue>(entity =>
        {
            entity.Property(e => e.RequestJson).HasColumnName("RequestJSON");
        });

        modelBuilder.Entity<RechargeRequest>(entity =>
        {
            entity.HasIndex(e => e.CustomerWalletSecondaryId, "IX_RechargeRequests_CustomerWalletSecondaryId");

            entity.HasIndex(e => e.ProviderId, "IX_RechargeRequests_ProviderId");

            entity.Property(e => e.RechargeRequestId).ValueGeneratedNever();
            entity.Property(e => e.AdminMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Commission).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CustomerDiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentGatewayAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SurchargeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SurchargeValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TaxValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WalletAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AdminWallet).WithMany(p => p.RechargeRequests).HasForeignKey(d => d.AdminWalletId);

            entity.HasOne(d => d.CustomerWallet).WithMany(p => p.RechargeRequestCustomerWallets).HasForeignKey(d => d.CustomerWalletId);

            entity.HasOne(d => d.CustomerWalletSecondary).WithMany(p => p.RechargeRequestCustomerWalletSecondaries).HasForeignKey(d => d.CustomerWalletSecondaryId);

            entity.HasOne(d => d.GatewayTransaction).WithMany(p => p.RechargeRequests).HasForeignKey(d => d.GatewayTransactionId);

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.RechargeRequests)
                .HasForeignKey(d => d.OperatorMasterServiceTypeId)
                .HasConstraintName("FK_RechargeRequests_OperatorMasterServiceTypes");

            entity.HasOne(d => d.Provider).WithMany(p => p.RechargeRequests).HasForeignKey(d => d.ProviderId);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.RechargeRequests).HasForeignKey(d => d.UserName);
        });

        modelBuilder.Entity<RechargeRequestDispute>(entity =>
        {
            entity.ToTable("RechargeRequestDispute");

            entity.HasIndex(e => e.OperatorMasterServiceTypeId, "IX_RechargeRequestDispute_OperatorMasterServiceTypeId");

            entity.HasIndex(e => e.ProviderId, "IX_RechargeRequestDispute_ProviderId");

            entity.HasIndex(e => e.RechargeRequestId, "IX_RechargeRequestDispute_RechargeRequestId");

            entity.HasIndex(e => e.UserName, "IX_RechargeRequestDispute_UserName");

            entity.Property(e => e.RechargeRequestDisputeId).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.RechargeRequestDisputes)
                .HasForeignKey(d => d.OperatorMasterServiceTypeId)
                .HasConstraintName("FK_RechargeRequestDispute_OperatorMasterServiceTypes");

            entity.HasOne(d => d.Provider).WithMany(p => p.RechargeRequestDisputes).HasForeignKey(d => d.ProviderId);

            entity.HasOne(d => d.RechargeRequest).WithMany(p => p.RechargeRequestDisputes).HasForeignKey(d => d.RechargeRequestId);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.RechargeRequestDisputes).HasForeignKey(d => d.UserName);
        });

        modelBuilder.Entity<RequestMapper>(entity =>
        {
            entity.HasKey(e => e.RequestId);
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.ToTable("Resident");

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

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.Residents)
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
            entity.ToTable("ResidentDocumentUploaded");

            entity.HasIndex(e => e.EntityTypeDetailId, "IX_ResidentDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.ResidentId, "IX_ResidentDocumentUploaded_ResidentId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.ResidentDocumentUploadeds).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentDocumentUploadeds).HasForeignKey(d => d.ResidentId);
        });

        modelBuilder.Entity<ResidentFamily>(entity =>
        {
            entity.ToTable("ResidentFamily");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFamilies)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFamily_Resident");
        });

        modelBuilder.Entity<ResidentFrequentlyEntry>(entity =>
        {
            entity.ToTable("ResidentFrequentlyEntry");

            entity.HasIndex(e => e.CabCompanyId, "IX_ResidentFrequentlyEntry_CabCompanyId");

            entity.HasIndex(e => e.DeliveryCompanyId, "IX_ResidentFrequentlyEntry_DeliveryCompanyId");

            entity.HasIndex(e => e.ResidentId, "IX_ResidentFrequentlyEntry_ResidentId");

            entity.HasIndex(e => e.VisitingHelpCategoryCompanyId, "IX_ResidentFrequentlyEntry_VisitingHelpCategoryCompanyId");

            entity.HasIndex(e => e.VisitingHelpCategoryId, "IX_ResidentFrequentlyEntry_VisitingHelpCategoryId");

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

            entity.HasOne(d => d.VisitingHelpCategoryCompany).WithMany(p => p.ResidentFrequentlyEntries).HasForeignKey(d => d.VisitingHelpCategoryCompanyId);

            entity.HasOne(d => d.VisitingHelpCategory).WithMany(p => p.ResidentFrequentlyEntries).HasForeignKey(d => d.VisitingHelpCategoryId);
        });

        modelBuilder.Entity<ResidentFrequentlyGuest>(entity =>
        {
            entity.ToTable("ResidentFrequentlyGuest");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentFrequentlyGuests)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentFrequentlyGuests_Resident");
        });

        modelBuilder.Entity<ResidentPet>(entity =>
        {
            entity.ToTable("ResidentPet");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentPets)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentPet_Resident");
        });

        modelBuilder.Entity<ResidentVehicle>(entity =>
        {
            entity.ToTable("ResidentVehicle");

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

            entity.HasOne(d => d.Resident).WithMany(p => p.ResidentVehicles)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResidentVehicle_Resident");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.Property(e => e.AdditionalUrl).HasMaxLength(200);
            entity.Property(e => e.BaseUrl).HasMaxLength(200);
            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.ClassText).HasMaxLength(100);
            entity.Property(e => e.Endpoint).HasMaxLength(200);
            entity.Property(e => e.FieldFifth).HasMaxLength(100);
            entity.Property(e => e.FieldFirst).HasMaxLength(100);
            entity.Property(e => e.FieldForth).HasMaxLength(100);
            entity.Property(e => e.FieldSecond).HasMaxLength(100);
            entity.Property(e => e.FieldThird).HasMaxLength(100);
            entity.Property(e => e.ResourceName).HasMaxLength(100);
            entity.Property(e => e.ResourceResponseParameterDetails).HasDefaultValue("");
            entity.Property(e => e.Verb).HasMaxLength(50);

            entity.HasOne(d => d.Provider).WithMany(p => p.Resources)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ResourceMapper>(entity =>
        {
            entity.HasKey(e => e.ResourceId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.ParentRoleId, "IX_Roles_ParentRoleId");

            entity.HasOne(d => d.ParentRole).WithMany(p => p.InverseParentRole).HasForeignKey(d => d.ParentRoleId);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.ToTable("RoleClaim");

            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaim).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Schema>(entity =>
        {
            entity.HasKey(e => e.Version).HasName("PK_HangFire_Schema");

            entity.ToTable("Schema", "HangFire");

            entity.Property(e => e.Version).ValueGeneratedNever();
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_HangFire_Server");

            entity.ToTable("Server", "HangFire");

            entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

            entity.Property(e => e.Id).HasMaxLength(200);
            entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
        });

        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.MobileImage).HasMaxLength(1000);
            entity.Property(e => e.OptionalImage).HasMaxLength(1000);
            entity.Property(e => e.ServiceCategoryLabel).HasMaxLength(200);
            entity.Property(e => e.ServiceCategoryName).HasMaxLength(100);
            entity.Property(e => e.WebImage).HasMaxLength(1000);
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.MobileImage).HasMaxLength(225);
            entity.Property(e => e.OptionalImage).HasMaxLength(225);
            entity.Property(e => e.ServiceLabel).HasMaxLength(200);
            entity.Property(e => e.ServiceName).HasMaxLength(200);
            entity.Property(e => e.WebImage).HasMaxLength(225);

            entity.HasOne(d => d.PlanProvider).WithMany(p => p.ServiceTypes).HasForeignKey(d => d.PlanProviderId);

            entity.HasOne(d => d.ServiceCategory).WithMany(p => p.ServiceTypes)
                .HasForeignKey(d => d.ServiceCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => new { e.Key, e.Value }).HasName("PK_HangFire_Set");

            entity.ToTable("Set", "HangFire");

            entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt").HasFilter("([ExpireAt] IS NOT NULL)");

            entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Value).HasMaxLength(256);
            entity.Property(e => e.ExpireAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.SettingValue).HasMaxLength(100);
        });

        modelBuilder.Entity<Society>(entity =>
        {
            entity.ToTable("Society");

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

            entity.HasIndex(e => e.SocietyId, "IX_SocietyBuilding_SocietyId");

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

        modelBuilder.Entity<SocietyDocumentUploaded>(entity =>
        {
            entity.ToTable("SocietyDocumentUploaded");

            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyDocumentUploaded_SocietyId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyDocumentUploadeds).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyDocumentUploadeds).HasForeignKey(d => d.SocietyId);
        });

        modelBuilder.Entity<SocietyFlat>(entity =>
        {
            entity.ToTable("SocietyFlat");

            entity.HasIndex(e => e.SocietyBuildingId, "IX_SocietyFlat_SocietyBuildingId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyFlat_SocietyId");

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

        modelBuilder.Entity<SocietyUser>(entity =>
        {
            entity.ToTable("SocietyUser");

            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyUser_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyId, "IX_SocietyUser_SocietyId");

            entity.HasIndex(e => e.UserId, "IX_SocietyUser_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyUsers).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.Society).WithMany(p => p.SocietyUsers).HasForeignKey(d => d.SocietyId);

            entity.HasOne(d => d.User).WithMany(p => p.SocietyUser).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SocietyUserDocumentUploaded>(entity =>
        {
            entity.ToTable("SocietyUserDocumentUploaded");

            entity.HasIndex(e => e.EntityTypeDetailId, "IX_SocietyUserDocumentUploaded_EntityTypeDetailId");

            entity.HasIndex(e => e.SocietyUserId, "IX_SocietyUserDocumentUploaded_SocietyUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.EntityTypeDetail).WithMany(p => p.SocietyUserDocumentUploadeds).HasForeignKey(d => d.EntityTypeDetailId);

            entity.HasOne(d => d.SocietyUser).WithMany(p => p.SocietyUserDocumentUploadeds).HasForeignKey(d => d.SocietyUserId);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.HasIndex(e => e.CountryId, "IX_State_CountryId");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
        });

        modelBuilder.Entity<State1>(entity =>
        {
            entity.HasKey(e => new { e.JobId, e.Id }).HasName("PK_HangFire_State");

            entity.ToTable("State", "HangFire");

            entity.HasIndex(e => e.CreatedAt, "IX_HangFire_State_CreatedAt");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Reason).HasMaxLength(100);

            entity.HasOne(d => d.Job).WithMany(p => p.State1s)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_HangFire_State_Job");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("SubscriptionPlan");

            entity.Property(e => e.AdminMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MobileImage).HasMaxLength(1000);
            entity.Property(e => e.Optional1).HasMaxLength(500);
            entity.Property(e => e.Optional2).HasMaxLength(500);
            entity.Property(e => e.Optional3).HasMaxLength(500);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.PlanTitle).HasMaxLength(200);
            entity.Property(e => e.WebImage).HasMaxLength(1000);

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.SubscriptionPlans).HasForeignKey(d => d.OperatorMasterServiceTypeId);
        });

        modelBuilder.Entity<SystemInfo>(entity =>
        {
            entity.ToTable("SystemInfo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AndroidAppVersion)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.ApiVersion).HasMaxLength(50);
            entity.Property(e => e.IosAppVersion).HasMaxLength(50);
            entity.Property(e => e.Message).HasMaxLength(250);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserName);

            entity.Property(e => e.UserName).ValueGeneratedNever();
            entity.Property(e => e.AccessToken).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.LastName).HasMaxLength(200);
            entity.Property(e => e.MiddleName).HasMaxLength(200);
            entity.Property(e => e.Otp)
                .HasMaxLength(10)
                .HasColumnName("OTP");
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Prefix).HasMaxLength(200);
            entity.Property(e => e.RefreshToken).HasMaxLength(1000);
            entity.Property(e => e.Role).HasMaxLength(100);
            entity.Property(e => e.RolesId).HasDefaultValue(0);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.VerificationCodeGenerationTime).HasColumnType("datetime");
            entity.Property(e => e.VerificationCodeValidationTime).HasColumnType("datetime");

            entity.HasOne(d => d.Roles).WithMany(p => p.User).HasForeignKey(d => d.RolesId);
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.ToTable("UserFavorite");

            entity.HasOne(d => d.ServiceType).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.ServiceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.UserFavorite).HasForeignKey(d => d.UserName);
        });

        modelBuilder.Entity<UserParent>(entity =>
        {
            entity.ToTable("UserParent");

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
            entity.ToTable("UserProfile");

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

        modelBuilder.Entity<UserRedeemCode>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CashbackAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinimumAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<UserReferralCode>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CashbackAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinimumAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<UserSubscriptionPlan>(entity =>
        {
            entity.ToTable("UserSubscriptionPlan");

            entity.Property(e => e.AdminMarginValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentGatewayAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.WalletAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AdminWallet).WithMany(p => p.UserSubscriptionPlans).HasForeignKey(d => d.AdminWalletId);

            entity.HasOne(d => d.CustomerWallet).WithMany(p => p.UserSubscriptionPlans).HasForeignKey(d => d.CustomerWalletId);

            entity.HasOne(d => d.GatewayTransaction).WithMany(p => p.UserSubscriptionPlans).HasForeignKey(d => d.GatewayTransactionId);

            entity.HasOne(d => d.OperatorMasterServiceType).WithMany(p => p.UserSubscriptionPlans).HasForeignKey(d => d.OperatorMasterServiceTypeId);

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.UserSubscriptionPlans).HasForeignKey(d => d.SubscriptionPlanId);
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

            entity.HasIndex(e => e.VisitingHelpCategoryId, "IX_VisitingHelpCategoryCompany_VisitingHelpCategoryId");

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

        modelBuilder.Entity<SocietyBuildingType>(entity =>
        {
            entity.ToTable("SocietyBuildingType");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<SocietyUserFlatWorkingHistory>(entity =>
        {
            entity.ToTable("SocietyUserFlatWorkingHistory");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.SocietyFlat).WithMany(p => p.SocietyUserFlatWorkingHistory)
                .HasForeignKey(d => d.SocietyFlatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyUserFlatWorkingHistory_SocietyFlat");

            entity.HasOne(d => d.SocietyUserProfile).WithMany(p => p.SocietyUserFlatWorkingHistory)
                .HasForeignKey(d => d.SocietyUserProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyUserFlatWorkingHistory_SocietyUserProfile");
        });

        modelBuilder.Entity<SocietyUserProfile>(entity =>
        {
            entity.ToTable("SocietyUserProfile");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AadhaarNumber).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PanNumber).HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UniqueCode).HasMaxLength(10);
            entity.Property(e => e.VehicleNo).HasMaxLength(100);

            entity.HasOne(d => d.SocietyBuildingType).WithMany(p => p.SocietyUserProfile)
                .HasForeignKey(d => d.SocietyBuildingTypeId)
                .HasConstraintName("FK_SocietyUserProfile_SocietyBuildingType");

            entity.HasOne(d => d.SocietyUser).WithMany(p => p.SocietyUserProfile)
                .HasForeignKey(d => d.SocietyUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyUserProfile_SocietyUserProfile");
        });

        modelBuilder.Entity<SocietyUserTimeSlot>(entity =>
        {
            entity.ToTable("SocietyUserTimeSlot");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DeletedBy).HasMaxLength(255);
            entity.Property(e => e.DeletedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.SocietyUserProfile).WithMany(p => p.SocietyUserTimeSlot)
                .HasForeignKey(d => d.SocietyUserProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocietyUserTimeSlot_SocietyUserProfile");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
