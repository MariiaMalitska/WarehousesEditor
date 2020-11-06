using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WarehousesEditor.Models
{
    public partial class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext()
        {
        }

        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<GoodsCategory> GoodsCategories { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<WarehouseGoods> WarehousesGoods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=warehouse;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.HasIndex(e => e.CategoryName)
                    .IsUnique();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.CurrencyId);

                entity.HasIndex(e => e.Code)
                    .IsUnique();

                entity.HasIndex(e => e.CurrencyName)
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateUpdated).HasColumnType("datetime");

                entity.Property(e => e.Rate).HasColumnType("money");
            });

            modelBuilder.Entity<Goods>(entity =>
            {
                entity.HasIndex(e => e.BarcodeNumber)
                    .IsUnique();

                entity.HasIndex(e => e.GoodsName)
                    .IsUnique();

                entity.Property(e => e.BarcodeNumber)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('00000000')");

                entity.Property(e => e.BaseCurrencyPrice).HasColumnType("money");

                entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

                entity.Property(e => e.GoodsName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasComputedColumnSql("([dbo].[ComputePrice]([CurrencyId],[BaseCurrencyPrice]))");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Goods_Currencies");
            });

            modelBuilder.Entity<GoodsCategory>(entity =>
            {
                entity.HasKey(e => new { e.GoodsId, e.CategoryId });

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.GoodsCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_GoodsCategories_Categories");

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.GoodsCategories)
                    .HasForeignKey(d => d.GoodsId)
                    .HasConstraintName("FK_GoodsCategories_Goods");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.HasKey(e => e.WarehouseId);

                entity.HasIndex(e => e.Address)
                    .IsUnique();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.WarehouseName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WarehouseGoods>(entity =>
            {
                entity.HasKey(e => new { e.WarehouseId, e.GoodsId });

                entity.HasOne(d => d.Goods)
                    .WithMany(p => p.WarehousesGoods)
                    .HasForeignKey(d => d.GoodsId)
                    .HasConstraintName("FK_WarehousesGoods_Goods");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.WarehousesGoods)
                    .HasForeignKey(d => d.WarehouseId)
                    .HasConstraintName("FK_WarehousesGoods_Warehouses");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
