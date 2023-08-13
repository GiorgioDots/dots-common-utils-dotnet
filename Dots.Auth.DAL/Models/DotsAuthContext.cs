using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Dots.Commons;
using Dots.Commons.DALs;

namespace Dots.Auth.DAL.Models;

public partial class DotsAuthContext : DbContext
{
    public DotsAuthContext(DbContextOptions<DotsAuthContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accounts> Accounts { get; set; }

    public virtual DbSet<AccountsApplications> AccountsApplications { get; set; }

    public virtual DbSet<AccountsTokens> AccountsTokens { get; set; }

    public virtual DbSet<Applications> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accounts>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<AccountsApplications>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.IdAccountNavigation).WithMany(p => p.AccountsApplications)
                .HasForeignKey(d => d.IdAccount)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountsApplications_Accounts");

            entity.HasOne(d => d.IdApplicationNavigation).WithMany(p => p.AccountsApplications)
                .HasForeignKey(d => d.IdApplication)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountsApplications_Applications");
        });

        modelBuilder.Entity<AccountsTokens>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(d => d.IdAccountNavigation).WithMany(p => p.AccountsTokens)
                .HasForeignKey(d => d.IdAccount)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountsTokens_Accounts");
        });

        modelBuilder.Entity<Applications>(entity =>
        {
            entity.Property(e => e.Audience)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Secret)
                .IsRequired()
                .HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    

    public async Task<int> Commit()
    {
        if (!ChangeTracker.HasChanges()) 
            return await base.SaveChangesAsync();

        foreach(var entity in ChangeTracker.Entries())
        {
            var upd = entity.Entity as BaseUpdatable;
            if (upd == null) continue;
            
            if (entity.State == EntityState.Added)
            {
                upd.Status = (int)eEntityStatus.OK;
                upd.DateCreate = DateTime.Now;
                upd.DateUpdate = DateTime.Now;
                continue;
            }
            if (entity.State == EntityState.Modified)
            {
                upd.DateUpdate = DateTime.Now;
                continue;
            }
            if (entity.State == EntityState.Deleted)
            {
                upd.DateUpdate = DateTime.Now;
                upd.Status = (int)eEntityStatus.DELETED;
                await upd.DeleteReferences(this);
                entity.State = EntityState.Modified;
                continue;
            }

        }
        return await base.SaveChangesAsync();
    }
}
