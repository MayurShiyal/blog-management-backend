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
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();

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
              .HasMaxLength(200);

            entity.Property(c => c.IsActive)
              .HasDefaultValue(true)
              .ValueGeneratedNever(); 

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
              .HasMaxLength(100);

            entity.Property(b => b.Slug)
              .IsRequired()
              .HasMaxLength(100);

            entity.HasIndex(b => b.Slug)
              .IsUnique()
              .HasDatabaseName("IX_Blogs_Slug");

            entity.Property(b => b.ShortDescription)
              .HasMaxLength(150);

            entity.Property(b => b.Content)
              .IsRequired();

            entity.Property(b => b.ThumbnailUrl)
              .HasMaxLength(200);

            entity.Property(b => b.Status)
              .HasConversion<int>()
              .HasDefaultValue(BlogStatus.Draft)
              .ValueGeneratedNever(); 
            entity.Property(b => b.IsDeleted)
              .HasDefaultValue(false)
              .ValueGeneratedNever();

            entity.Property(b => b.DeletedAt)
              .HasColumnType("timestamp with time zone");

            entity.Property(b => b.RejectionReason)
              .HasMaxLength(100)
              .IsRequired(false);

            entity.Property(b => b.CreatedAt)
              .HasColumnType("timestamp with time zone");

            entity.Property(b => b.UpdatedAt)
              .HasColumnType("timestamp with time zone");

            entity.Property(b => b.PublishedAt)
              .HasColumnType("timestamp with time zone");

            entity.HasOne(b => b.Author)
              .WithMany(u => u.Blogs)
              .HasForeignKey(b => b.AuthorId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // ── BlogCategory (junction table) configuration ──────────────────
        modelBuilder.Entity<BlogCategory>(entity =>
        {
            // Composite PK
            entity.HasKey(bc => new { bc.BlogId, bc.CategoryId });

            entity.HasOne(bc => bc.Blog)
              .WithMany(b => b.BlogCategories)
              .HasForeignKey(bc => bc.BlogId)
              .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bc => bc.Category)
              .WithMany(c => c.BlogCategories)
              .HasForeignKey(bc => bc.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(bc => bc.CategoryId)
              .HasDatabaseName("IX_BlogCategories_CategoryId");
        });
    }
}