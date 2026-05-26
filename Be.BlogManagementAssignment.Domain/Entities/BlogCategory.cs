namespace Be.BlogManagementAssignment.Domain.Entities;

/// <summary>
/// Junction/pivot entity that links a Blog to one or more Categories
/// in a many-to-many relationship.
/// </summary>
public class BlogCategory
{
    public Guid BlogId { get; set; }

    public int CategoryId { get; set; }

    // Navigation properties
    public Blog Blog { get; set; } = default!;

    public Category Category { get; set; } = default!;
}
