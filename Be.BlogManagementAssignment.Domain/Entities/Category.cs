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

    /// <summary>Many-to-many: a category can contain multiple blogs.</summary>
    public ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}
