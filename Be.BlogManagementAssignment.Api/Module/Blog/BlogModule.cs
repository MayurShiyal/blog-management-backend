using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

namespace Be.BlogManagementAssignment.Api.Module.Blog;

/// <summary>
/// Registers all blog-related Minimal API endpoints under /api/blogs.
///
///   GET    /api/blogs              → GetBlogsEndPoint        (anonymous | author | admin)
///   GET    /api/blogs/{id}         → GetBlogByIdEndPoint     (anonymous | author | admin)
///   POST   /api/blogs              → CreateBlogEndPoint      (author only)
///   PUT    /api/blogs/{id}         → UpdateBlogEndPoint      (author | admin — content only)
///   PATCH  /api/blogs/{id}/status  → UpdateBlogStatusEndPoint (admin only — approve/reject)
///   DELETE /api/blogs/{id}         → DeleteBlogEndPoint      (author | admin)
/// </summary>
public class BlogModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/blogs")
            .WithTags("Blog");

        group.MapEndPoints<GetBlogsEndPoint>();          // GET    /api/blogs
        group.MapEndPoints<GetBlogByIdEndPoint>();       // GET    /api/blogs/{id}
        group.MapEndPoints<CreateBlogEndPoint>();        // POST   /api/blogs
        group.MapEndPoints<UpdateBlogEndPoint>();        // PUT    /api/blogs/{id}
        group.MapEndPoints<UpdateBlogStatusEndPoint>();  // PATCH  /api/blogs/{id}/status
        group.MapEndPoints<DeleteBlogEndPoint>();        // DELETE /api/blogs/{id}
    }
}
