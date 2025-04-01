using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ArchiveHist.Model
{
    public partial class ArchiveContext : DbContext
    {
        public ArchiveContext()
        {
        }

        public ArchiveContext(DbContextOptions<ArchiveContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AudioFile> AudioFiles { get; set; } = null!;
        public virtual DbSet<Collection> Collections { get; set; } = null!;
        public virtual DbSet<Delancey> Delanceys { get; set; } = null!;
        public virtual DbSet<Map> Maps { get; set; } = null!;
        public virtual DbSet<Oversized> Oversizeds { get; set; } = null!;
        public virtual DbSet<Photo> Photos { get; set; } = null!;
        public virtual DbSet<PoisonBook> PoisonBooks { get; set; } = null!;
        public virtual DbSet<ReportsPub> ReportsPubs { get; set; } = null!;
        public virtual DbSet<Research> Researches { get; set; } = null!;
        public virtual DbSet<Trunk> Trunks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=dpdalidsql02.city.phila.local;Database=DPD_ArchiveHist;Encrypt=false;TrustServerCertificate=True;Trusted_Connection=True;User Id=DPD_ArchiveHist_dev_app;Password=F0r!PlanningDevelopment");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AudioFile>(entity =>
            {
                entity.HasKey(e => e.AId)
                    .HasName("PK_aid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.AudioFiles)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK_Cid3");
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PK_cid");
            });

            modelBuilder.Entity<Delancey>(entity =>
            {
                entity.HasKey(e => e.DId)
                    .HasName("PK_did");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Delanceys)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK_c_id");
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.HasKey(e => e.MId)
                    .HasName("PK_mid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Maps)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK__cid");
            });

            modelBuilder.Entity<Oversized>(entity =>
            {
                entity.HasKey(e => e.OId)
                    .HasName("PK_oid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Oversizeds)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("F_K_cid");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasKey(e => e.PId)
                    .HasName("PK_pid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK_ci_d");
            });

            modelBuilder.Entity<PoisonBook>(entity =>
            {
                entity.HasKey(e => e.PId)
                    .HasName("P_ID");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.PoisonBooks)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK__cid1");
            });

            modelBuilder.Entity<ReportsPub>(entity =>
            {
                entity.HasKey(e => e.RpId)
                    .HasName("PK_ID");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.ReportsPubs)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK__cid2");
            });

            modelBuilder.Entity<Research>(entity =>
            {
                entity.HasKey(e => e.RId)
                    .HasName("PK_rid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Researches)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK_Cid");
            });

            modelBuilder.Entity<Trunk>(entity =>
            {
                entity.HasKey(e => e.TId)
                    .HasName("PK_tid");

                entity.HasOne(d => d.CIdNavigation)
                    .WithMany(p => p.Trunks)
                    .HasForeignKey(d => d.CId)
                    .HasConstraintName("FK_ci__d");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
