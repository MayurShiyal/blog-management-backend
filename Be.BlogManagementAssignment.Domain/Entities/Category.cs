namespace Be.BlogManagementAssignment.Domain.Entities;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation — one Category → many Blogs
    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
