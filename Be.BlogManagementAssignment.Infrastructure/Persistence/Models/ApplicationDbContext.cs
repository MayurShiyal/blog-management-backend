using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Persistence.Models;

public class ApplicationDbContext : DbContext
{
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
      {
      }

      public DbSet<User> Users => Set<User>();
      public DbSet<Category> Categories => Set<Category>();
      public DbSet<Blog> Blogs => Set<Blog>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);

            // ── Category configuration ──────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                  entity.HasKey(c => c.Id);

                  entity.Property(c => c.Id)
                    .UseIdentityByDefaultColumn();

                  entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                  entity.Property(c => c.Slug)
                    .IsRequired()
                    .HasMaxLength(150);

                  entity.HasIndex(c => c.Slug)
                    .IsUnique()
                    .HasDatabaseName("IX_Categories_Slug");

                  entity.Property(c => c.Description)
                    .HasMaxLength(500);

                  entity.Property(c => c.IsActive)
                    .HasDefaultValue(true);

                  entity.Property(c => c.CreatedAt)
                    .HasColumnType("timestamp with time zone");

                  entity.Property(c => c.UpdatedAt)
                    .HasColumnType("timestamp with time zone");
            });

            // ── Blog configuration ──────────────────────────────────────────
            modelBuilder.Entity<Blog>(entity =>
            {
                  entity.HasKey(b => b.Id);

                  entity.Property(b => b.Id)
                    .HasColumnType("uuid");

                  entity.Property(b => b.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                  entity.Property(b => b.Slug)
                    .IsRequired()
                    .HasMaxLength(300);

                  entity.HasIndex(b => b.Slug)
                    .IsUnique()
                    .HasDatabaseName("IX_Blogs_Slug");

                  entity.Property(b => b.ShortDescription)
                    .HasMaxLength(500);

                  entity.Property(b => b.Content)
                    .IsRequired();

                  entity.Property(b => b.ThumbnailUrl)
                    .HasMaxLength(2000);

                  entity.Property(b => b.Status)
                    .HasConversion<int>()
                    .HasDefaultValue(BlogStatus.Draft);

                  // RejectionReason: nullable, max 1000 chars, set only on Rejected status
                  entity.Property(b => b.RejectionReason)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                  entity.Property(b => b.CreatedAt)
                    .HasColumnType("timestamp with time zone");

                  entity.Property(b => b.UpdatedAt)
                    .HasColumnType("timestamp with time zone");

                  entity.Property(b => b.PublishedAt)
                    .HasColumnType("timestamp with time zone");

                  // ── Relationships ──────────────────────────────────────────
                  entity.HasOne(b => b.Category)
                    .WithMany(c => c.Blogs)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                  entity.HasOne(b => b.Author)
                    .WithMany(u => u.Blogs)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
      }
}
