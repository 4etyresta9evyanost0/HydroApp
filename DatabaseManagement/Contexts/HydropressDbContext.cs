using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace HydroApp
{

    public partial class HydropressDbContext : DbContext
    {
        string _connectionString;
        public HydropressDbContext(string connectionString
            = "Data Source=DESKTOP-8L2M3KV\\SQLEXPRESS01;Initial Catalog=HydropressDB;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True")
        {
            _connectionString = connectionString;
        }

        public HydropressDbContext(DbContextOptions<HydropressDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Batch> Batches { get; set; }

        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Commission> Commissions { get; set; }

        public virtual DbSet<CommissionDetail> CommissionDetails { get; set; }

        public virtual DbSet<Construction> Constructions { get; set; }

        public virtual DbSet<Designer> Designers { get; set; }

        public virtual DbSet<Detail> Details { get; set; }

        public virtual DbSet<DetailsForProduction> DetailsForProductions { get; set; }

        public virtual DbSet<Employee> Employees { get; set; }

        public virtual DbSet<Foreman> Foremen { get; set; }

        public virtual DbSet<Material> Materials { get; set; }

        public virtual DbSet<MaterialsForDetail> MaterialsForDetails { get; set; }

        public virtual DbSet<Production> Productions { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<Supply> Supplies { get; set; }

        public virtual DbSet<SupplyDetail> SupplyDetails { get; set; }

        public virtual DbSet<Workshop> Workshops { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(Settings.Default.MainDbConnectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Batch>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_BatchId");

                entity.ToTable("Batch");

                entity.Property(e => e.CompletionDate).HasColumnType("date");
                entity.Property(e => e.RequestDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("date");

                entity.HasOne(d => d.DetailNavigation).WithMany(p => p.Batches)
                    .HasForeignKey(d => d.Detail)
                    .HasConstraintName("FK_Batch_Detail");

                entity.HasOne(d => d.ForemanNavigation).WithMany(p => p.Batches)
                    .HasForeignKey(d => d.Foreman)
                    .HasConstraintName("FK_Batch_Foremen");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ClientId");

                entity.Property(e => e.Adress).HasMaxLength(192);
                entity.Property(e => e.Inn).HasColumnName("INN");
                entity.Property(e => e.Kpp).HasColumnName("KPP");
                entity.Property(e => e.Name).HasMaxLength(72);
                entity.Property(e => e.Ogrn).HasColumnName("OGRN");
            });

            modelBuilder.Entity<Commission>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_CommisionId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CommissionDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("date");
                entity.Property(e => e.ExecutionDate).HasColumnType("date");

                entity.HasOne(d => d.ClientNavigation).WithMany(p => p.Commissions)
                    .HasForeignKey(d => d.Client)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Commission_Client");
            });

            modelBuilder.Entity<CommissionDetail>(entity =>
            {
                entity.HasKey(e => new { e.IdCommission, e.IdConstruction }).HasName("PK_ConstructionCommissionId");

                entity.HasOne(d => d.IdCommissionNavigation).WithMany(p => p.CommissionDetails)
                    .HasForeignKey(d => d.IdCommission)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConstructionCommission_Commision");

                entity.HasOne(d => d.IdConstructionNavigation).WithMany(p => p.CommissionDetails)
                    .HasForeignKey(d => d.IdConstruction)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ConstructionCommission_Construction");
            });

            modelBuilder.Entity<Construction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ConstructionId");

                entity.Property(e => e.Name).HasMaxLength(32);
                entity.Property(e => e.Price).HasColumnType("money");

                entity.HasOne(d => d.DeveloperNavigation).WithMany(p => p.Constructions)
                    .HasForeignKey(d => d.Developer)
                    .HasConstraintName("FK_Constructions_Designer");
            });

            modelBuilder.Entity<Designer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_DesignerId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation).WithOne(p => p.Designer)
                    .HasForeignKey<Designer>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Designer_Employee");
            });

            modelBuilder.Entity<Detail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_DetailId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Purpose).HasMaxLength(24);

                entity.HasOne(d => d.IdNavigation).WithOne(p => p.Detail)
                    .HasForeignKey<Detail>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Detail_Construction");
            });

            modelBuilder.Entity<DetailsForProduction>(entity =>
            {
                entity.HasKey(e => new { e.IdProduction, e.IdDetail }).HasName("PK_DetailProductionId");

                entity.HasOne(d => d.IdDetailNavigation).WithMany(p => p.DetailsForProductions)
                    .HasForeignKey(d => d.IdDetail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DetailProduction_Detail");

                entity.HasOne(d => d.IdProductionNavigation).WithMany(p => p.DetailsForProductions)
                    .HasForeignKey(d => d.IdProduction)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DetailProduction_Production");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_EmployeeID");

                entity.Property(e => e.Adress).HasMaxLength(192);
                entity.Property(e => e.Firstname).HasMaxLength(56);
                entity.Property(e => e.Occupation).HasMaxLength(56);
                entity.Property(e => e.Patronym).HasMaxLength(64);
                entity.Property(e => e.Surname).HasMaxLength(56);
            });

            modelBuilder.Entity<Foreman>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ForemanId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.IdNavigation).WithOne(p => p.Foreman)
                    .HasForeignKey<Foreman>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Foreman_Employee");

                entity.HasOne(d => d.WorkshopNavigation).WithMany(p => p.Foremen)
                    .HasForeignKey(d => d.Workshop)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Foreman_Workshop");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_MaterialId");

                entity.Property(e => e.Name).HasMaxLength(24);
                entity.Property(e => e.Type).HasMaxLength(24);
                entity.Property(e => e.Unit).HasMaxLength(16);
            });

            modelBuilder.Entity<MaterialsForDetail>(entity =>
            {
                entity.HasKey(e => new { e.IdDetail, e.IdMaterial }).HasName("PK_MaterialDetailId");

                entity.HasOne(d => d.IdDetailNavigation).WithMany(p => p.MaterialsForDetails)
                    .HasForeignKey(d => d.IdDetail)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialDetail_Detail");

                entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.MaterialsForDetails)
                    .HasForeignKey(d => d.IdMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialDetail_Material");
            });

            modelBuilder.Entity<Production>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_ProductionId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.BrandName).HasMaxLength(32);
                entity.Property(e => e.Type).HasMaxLength(32);

                entity.HasOne(d => d.IdNavigation).WithOne(p => p.Production)
                    .HasForeignKey<Production>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Production_Construction");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_SupplierId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Adress).HasMaxLength(192);
                entity.Property(e => e.Inn).HasColumnName("INN");
                entity.Property(e => e.Kpp).HasColumnName("KPP");
                entity.Property(e => e.Name).HasMaxLength(72);
                entity.Property(e => e.Ogrn).HasColumnName("OGRN");
            });

            modelBuilder.Entity<Supply>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_SupplyId");

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.CommissionDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("date");
                entity.Property(e => e.SupplyDate).HasColumnType("date");

                entity.HasOne(d => d.SupplierNavigation).WithMany(p => p.Supplies)
                    .HasForeignKey(d => d.Supplier)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Supply_Supplier");
            });

            modelBuilder.Entity<SupplyDetail>(entity =>
            {
                entity.HasKey(e => new { e.IdSupply, e.IdMaterial }).HasName("PK_SupplyMaterialId");

                entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.SupplyDetails)
                    .HasForeignKey(d => d.IdMaterial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplyMaterial_Material");

                entity.HasOne(d => d.IdSupplyNavigation).WithMany(p => p.SupplyDetails)
                    .HasForeignKey(d => d.IdSupply)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplyMaterial_Supply");
            });

            modelBuilder.Entity<Workshop>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_WorkshopId");

                entity.Property(e => e.Name).HasMaxLength(24);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}