using Be.BlogManagementAssignment.Domain.Enums;

namespace Be.BlogManagementAssignment.Domain.Entities;

public class Blog : BaseEntity
{
    public string Title { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? ShortDescription { get; set; }

    public string Content { get; set; } = default!;

    public string? ThumbnailUrl { get; set; }

    public BlogStatus Status { get; set; } = BlogStatus.Draft;

    public string? RejectionReason { get; set; }

    public Guid AuthorId { get; set; }

    public DateTime? PublishedAt { get; set; }

    // Navigation properties
    public User Author { get; set; } = default!;

    /// <summary>Many-to-many: a blog can belong to multiple categories.</summary>
    public ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}
