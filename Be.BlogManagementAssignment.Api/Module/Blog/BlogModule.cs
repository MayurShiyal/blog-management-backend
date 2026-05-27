using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.DeleteBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogById;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.GetBlogs;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlogStatus;

namespace Be.BlogManagementAssignment.Api.Module.Blog;

public class BlogModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/blogs")
            .WithTags("Blog");

        group.MapEndPoints<GetBlogsEndPoint>();          
        group.MapEndPoints<GetBlogByIdEndPoint>();       
        group.MapEndPoints<CreateBlogEndPoint>();      
        group.MapEndPoints<UpdateBlogEndPoint>();        
        group.MapEndPoints<UpdateBlogStatusEndPoint>();  
        group.MapEndPoints<DeleteBlogEndPoint>();       
    }
}
