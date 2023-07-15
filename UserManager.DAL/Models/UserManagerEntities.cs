using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserManager.DTO;

namespace UserManager.DAL.Models;

public partial class UserManagerEntities : DbContext
{
    public static Dictionary<Type, List<PropertyInfo?>> types = new Dictionary<Type, List<PropertyInfo?>>();

    public UserManagerEntities(DbContextOptions<UserManagerEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<Tags> Tags { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<UsersTags> UsersTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tags>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");
            entity.Property(e => e.FastLoginKey).HasMaxLength(300);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<UsersTags>(entity =>
        {
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateUpdate).HasColumnType("datetime");

            entity.HasOne(d => d.IdTagNavigation).WithMany(p => p.UsersTags)
                .HasForeignKey(d => d.IdTag)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsersTags_Tags");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UsersTags)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsersTags_Users");
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
            var type = entity.Entity.GetType();
            if (type == null) continue;
            PropertyInfo? statusProp = null;
            PropertyInfo? duProp = null;
            PropertyInfo? dcProp = null;
            if (!types.ContainsKey(type))
            {
                statusProp = type.GetProperty("Status");
                duProp = type.GetProperty("DateUpdate");
                dcProp = type.GetProperty("DateCreate");
                types.Add(type, new List<PropertyInfo?>() { statusProp, duProp, dcProp });
            }
            else
            {
                statusProp = types[type].FirstOrDefault(k => k.Name == "Status");
                duProp = types[type].FirstOrDefault(k => k.Name == "DateUpdate");
                dcProp = types[type].FirstOrDefault(k => k.Name == "DateCreate");
            }
            
            if (entity.State == EntityState.Added)
            {
                if(statusProp != null)
                {
                    statusProp.SetValue(entity.Entity, (int)eEntityStatus.OK);
                }
                if (duProp != null)
                {
                    duProp.SetValue(entity.Entity, DateTime.Now);
                }
                if (dcProp != null)
                {
                    dcProp.SetValue(entity.Entity, DateTime.Now);
                }
                continue;
            }
            if (entity.State == EntityState.Modified)
            {
                if (duProp != null)
                {
                    duProp.SetValue(entity.Entity, DateTime.Now);
                }
                continue;
            }
            if (entity.State == EntityState.Deleted)
            {
                if (duProp != null)
                {
                    duProp.SetValue(entity.Entity, DateTime.Now);
                }
                if (statusProp != null)
                {
                    statusProp.SetValue(entity.Entity, (int)eEntityStatus.DELETED);
                }
                MethodInfo? methodInfo = type.GetMethod("DeleteReferences");
                if (methodInfo != null)
                {
                    methodInfo.Invoke(entity.Entity, new object[] { this });
                }
                entity.State = EntityState.Modified;
                continue;
            }

        }
        return await base.SaveChangesAsync();
    }
}
