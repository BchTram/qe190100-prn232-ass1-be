using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace TaskTrack.Repo.Models;

public partial class Prn232PostgresContext : DbContext
{
    private readonly ILogger<Prn232PostgresContext>? _logger;

    public Prn232PostgresContext()
    {
    }

    public Prn232PostgresContext(DbContextOptions<Prn232PostgresContext> options)
        : base(options)
    {
    }

    public Prn232PostgresContext(
        DbContextOptions<Prn232PostgresContext> options,
        ILogger<Prn232PostgresContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<SystemAccount> SystemAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("Department_pkey");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentDescription).HasMaxLength(300);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("Project_pkey");

            entity.ToTable("Project");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValue((short)0);

            entity.HasOne(d => d.Department).WithMany(p => p.Projects)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_Department");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("Tag_pkey");

            entity.ToTable("Tag");

            entity.HasIndex(e => e.TagName, "Tag_TagName_key").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.Color).HasMaxLength(7);
            entity.Property(e => e.TagName).HasMaxLength(50);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("Task_pkey");

            entity.ToTable("Task");

            entity.Property(e => e.TaskId).HasColumnName("TaskID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Priority).HasDefaultValue((short)1);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Status).HasDefaultValue((short)0);
            entity.Property(e => e.Title).HasMaxLength(300);
            entity.Property(e => e.CreatedByAccountId).HasColumnName("CreatedByAccountID");

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Task_Project");

            entity.HasOne(d => d.CreatedByAccount).WithMany(p => p.CreatedTasks)
                .HasForeignKey(d => d.CreatedByAccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Task_SystemAccount");

            entity.HasMany(d => d.Tags).WithMany(p => p.Tasks)
                .UsingEntity<Dictionary<string, object>>(
                    "TaskTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TaskTag_Tag"),
                    l => l.HasOne<Task>().WithMany()
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TaskTag_Task"),
                    j =>
                    {
                        j.HasKey("TaskId", "TagId");
                        j.ToTable("TaskTag");
                        j.IndexerProperty<int>("TaskId").HasColumnName("TaskID");
                        j.IndexerProperty<int>("TagId").HasColumnName("TagID");
                    });
        });

        modelBuilder.Entity<SystemAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("SystemAccount_pkey");
            entity.ToTable("SystemAccount");
            entity.HasIndex(e => e.Email, "SystemAccount_Email_key").IsUnique();
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(320).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(
                acceptAllChangesOnSuccess,
                cancellationToken);
        }
        catch (Exception ex)
        {
            var pendingChanges = string.Join(
                ", ",
                ChangeTracker.Entries()
                    .Where(entry => entry.State != EntityState.Unchanged)
                    .Select(entry => $"{entry.Metadata.ClrType.Name}:{entry.State}"));

            _logger?.LogError(
                ex,
                "SaveChangesAsync failed. ExceptionType={ExceptionType}, Message={Message}, InnerException={InnerException}, StackTrace={StackTrace}, PendingChanges={PendingChanges}",
                ex.GetType().FullName,
                ex.Message,
                ex.InnerException?.ToString(),
                ex.StackTrace,
                pendingChanges);

            throw;
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
