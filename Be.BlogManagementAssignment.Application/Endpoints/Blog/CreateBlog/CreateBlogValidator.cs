using FluentValidation;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public class CreateBlogValidator : AbstractValidator<CreateBlogRequest>
{
    public CreateBlogValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(300).WithMessage("Slug must not exceed 300 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug may only contain lowercase letters, digits, and hyphens.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters.")
            .When(x => x.ShortDescription is not null);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2000).WithMessage("Thumbnail URL must not exceed 2000 characters.")
            .When(x => x.ThumbnailUrl is not null);

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("At least one category must be selected.")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("All category IDs must be positive integers.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate category IDs are not allowed.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid blog status.");
    }
}
