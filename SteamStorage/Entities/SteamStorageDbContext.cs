﻿using Microsoft.EntityFrameworkCore;

namespace SteamStorage.Entities;

public partial class SteamStorageDbContext : DbContext
{
    public SteamStorageDbContext()
    {
    }

    public SteamStorageDbContext(DbContextOptions<SteamStorageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Archive> Archives { get; set; }

    public virtual DbSet<ArchiveGroup> ArchiveGroups { get; set; }

    public virtual DbSet<PriceDynamic> PriceDynamics { get; set; }

    public virtual DbSet<Remain> Remains { get; set; }

    public virtual DbSet<RemainGroup> RemainGroups { get; set; }

    public virtual DbSet<Skin> Skins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=D:\\Programming\\Projects\\SteamStorageFolder\\SteamStorage\\SteamStorage\\DataBase\\SteamStorageDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Archive>(entity =>
        {
            entity.ToTable("Archive");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostPurchase).HasColumnName("cost_purchase");
            entity.Property(e => e.CostSold).HasColumnName("cost_sold");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.DatePurchase).HasColumnName("date_purchase");
            entity.Property(e => e.DateSold).HasColumnName("date_sold");
            entity.Property(e => e.IdGroup)
                .HasDefaultValueSql("1")
                .HasColumnName("id_group");
            entity.Property(e => e.IdSkin).HasColumnName("id_skin");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Archives)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdSkinNavigation).WithMany(p => p.Archives)
                .HasForeignKey(d => d.IdSkin)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArchiveGroup>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<PriceDynamic>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostUpdate).HasColumnName("cost_update");
            entity.Property(e => e.DateUpdate).HasColumnName("date_update");
            entity.Property(e => e.IdRemain).HasColumnName("id_remain");

            entity.HasOne(d => d.IdRemainNavigation).WithMany(p => p.PriceDynamics).HasForeignKey(d => d.IdRemain);
        });

        modelBuilder.Entity<Remain>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostPurchase).HasColumnName("cost_purchase");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.DatePurchase).HasColumnName("date_purchase");
            entity.Property(e => e.IdGroup)
                .HasDefaultValueSql("1")
                .HasColumnName("id_group");
            entity.Property(e => e.IdSkin).HasColumnName("id_skin");

            entity.HasOne(d => d.IdGroupNavigation).WithMany(p => p.Remains)
                .HasForeignKey(d => d.IdGroup)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.IdSkinNavigation).WithMany(p => p.Remains)
                .HasForeignKey(d => d.IdSkin)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RemainGroup>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Skin>(entity =>
        {
            entity.HasIndex(e => e.Title, "IX_Skins_title").IsUnique();

            entity.HasIndex(e => e.Url, "IX_Skins_url").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
