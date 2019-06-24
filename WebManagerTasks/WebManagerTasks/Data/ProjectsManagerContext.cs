using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebManagerTasks.Data.Models;

namespace WebManagerTasks
{
    public partial class ProjectsManagerContext : DbContext
    {
        public ProjectsManagerContext()
        {
        }

        public ProjectsManagerContext(DbContextOptions<ProjectsManagerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTask> ProjectsTasks { get; set; }
        public virtual DbSet<MyTask> Tasks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserProject> UsersProjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=localhost;Database=ProjectsManager;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateCreate)
                    .HasColumnName("date_create")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IdTask).HasColumnName("id_task");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.HasOne(d => d.IdTaskNavigation)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.IdTask)
                    .HasConstraintName("FK_Comments_Tasks");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.IdProject);

                entity.Property(e => e.IdProject).HasColumnName("id_project");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.ToTable("Projects_tasks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateStart)
                    .HasColumnName("date_start")
                    .HasColumnType("datetime");

                entity.Property(e => e.IdProject).HasColumnName("id_project");

                entity.Property(e => e.IdTask).HasColumnName("id_task");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdProjectNavigation)
                    .WithMany(p => p.ProjectsTasks)
                    .HasForeignKey(d => d.IdProject)
                    .HasConstraintName("FK_Projects_tasks_Projects");

                entity.HasOne(d => d.IdTaskNavigation)
                    .WithMany(p => p.ProjectsTasks)
                    .HasForeignKey(d => d.IdTask)
                    .HasConstraintName("FK_Projects_tasks_Tasks");
            });

            modelBuilder.Entity<MyTask>(entity =>
            {
                entity.HasKey(e => e.IdTask);

                entity.Property(e => e.IdTask).HasColumnName("id_task");

                entity.Property(e => e.DateEnd)
                    .HasColumnName("date_end")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateStart)
                    .HasColumnName("date_start")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasMaxLength(20);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(150);

                entity.Property(e => e.Salt)
                    .HasColumnName("salt")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserProject>(entity =>
            {
                entity.ToTable("Users_projects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdProject).HasColumnName("id_project");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.HasOne(d => d.IdProjectNavigation)
                    .WithMany(p => p.UsersProjects)
                    .HasForeignKey(d => d.IdProject)
                    .HasConstraintName("FK_Users_projects_Projects");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UsersProjects)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Users_projects_Users");
            });
        }
    }
}
