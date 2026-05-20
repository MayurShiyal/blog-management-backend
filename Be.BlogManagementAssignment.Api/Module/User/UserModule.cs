using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;
using Be.BlogManagementAssignment.Application.Endpoints.User.VerifyEmail;

namespace Be.BlogManagementAssignment.Api.Module.User;

public class UserModule : IModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app
            .MapGroup("/api/user")
            .WithTags("User");

        group.MapEndPoints<UserRegistrationEndPoint>();
        group.MapEndPoints<UserLoginEndPoint>();
        group.MapEndPoints<VerifyEmailEndPoint>();
        group.MapEndPoints<ForgotPasswordEndPoint>();
        group.MapEndPoints<ResetPasswordEndPoint>();
    }
}
