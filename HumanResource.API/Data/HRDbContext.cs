using System;
using System.Collections.Generic;
using HumanResource.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Data;

public partial class HRDbContext : DbContext
{
    public HRDbContext()
    {
    }

    public HRDbContext(DbContextOptions<HRDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobHistory> JobHistories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.CountryId).IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Countries).HasConstraintName("FK_countries_region_id");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasOne(d => d.Location).WithMany(p => p.Departments).HasConstraintName("FK_departments_location_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.Departments).HasConstraintName("FK__departmen__manag__5EBF139D");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees).HasConstraintName("FK_employees_department_id");

            entity.HasOne(d => d.Job).WithMany(p => p.Employees)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_employees_job_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager).HasConstraintName("FK_employees_manager_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees).HasConstraintName("FK_Employees_Roles");
        });

        modelBuilder.Entity<JobHistory>(entity =>
        {
            entity.HasOne(d => d.Department).WithMany(p => p.JobHistories).HasConstraintName("FK_job_history_department_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.JobHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_job_history_employee_id");

            entity.HasOne(d => d.Job).WithMany(p => p.JobHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_job_history_job_id");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.Property(e => e.CountryId).IsFixedLength();

            entity.HasOne(d => d.Country).WithMany(p => p.Locations).HasConstraintName("FK_locations_country_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A255F693B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
