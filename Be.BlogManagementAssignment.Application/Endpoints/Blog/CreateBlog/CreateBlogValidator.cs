using FluentValidation;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;

public class CreateBlogValidator : AbstractValidator<CreateBlogRequest>
{
    public CreateBlogValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Blog title is required.")
            .MinimumLength(3).WithMessage("Blog title must be at least 3 characters.")
            .MaximumLength(250).WithMessage("Blog title must not exceed 250 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(300).WithMessage("Slug must not exceed 300 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must contain only lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description must not exceed 500 characters.")
            .When(x => x.ShortDescription is not null);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Blog content is required.");

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(2000).WithMessage("Thumbnail URL must not exceed 2000 characters.")
            .When(x => x.ThumbnailUrl is not null);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category must be selected.");
    }
}
