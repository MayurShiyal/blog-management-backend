namespace Be.BlogManagementAssignment.Domain.Entities;

public class BlogCategory
{
    public Guid BlogId { get; set; }

    public int CategoryId { get; set; }

    public Blog Blog { get; set; } = default!;

    public Category Category { get; set; } = default!;
}
