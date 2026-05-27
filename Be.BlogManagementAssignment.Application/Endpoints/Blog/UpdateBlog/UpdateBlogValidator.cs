using FluentValidation;

namespace Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;

public class UpdateBlogValidator : AbstractValidator<UpdateBlogRequest>
{
    public UpdateBlogValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .When(x => x.Title is not null);

        RuleFor(x => x.Slug)
            .MaximumLength(100)
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must be lowercase, alphanumeric, and hyphen-separated.")
            .When(x => x.Slug is not null);

        RuleFor(x => x.ShortDescription)
            .MaximumLength(150)
            .When(x => x.ShortDescription is not null);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(200)
            .When(x => x.ThumbnailUrl is not null);
    }
}
