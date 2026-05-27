namespace Be.BlogManagementAssignment.Domain.Entities;

public class Category : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<BlogCategory> BlogCategories { get; set; } = new List<BlogCategory>();
}
