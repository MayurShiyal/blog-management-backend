using System.Text;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.CreateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Blog.UpdateBlog;
using Be.BlogManagementAssignment.Application.Endpoints.Category.AddCategories;
using Be.BlogManagementAssignment.Application.Endpoints.Category.UpdateCategories;
using Be.BlogManagementAssignment.Application.Endpoints.User.ForgotPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.ResetPassword;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserLogin;
using Be.BlogManagementAssignment.Application.Endpoints.User.UserRegistration;
using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Application.Interfaces.Services;
using Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;
using Be.BlogManagementAssignment.Infrastructure.Implementations.Services;
using Be.BlogManagementAssignment.Infrastructure.Implementations.Utilities;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Be.BlogManagementAssignment.Api.Extentions;

public static class ServiceExtention
{
    public static IServiceCollection RegisterServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Database ──────────────────────────────────────────────────────
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // ── CORS ──────────────────────────────────────────────────────────
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200",
                        "https://localhost:4200"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        // ── JWT Authentication ─────────────────────────────────────────────
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey  = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtSection["Issuer"]   ?? "BlogManagementApi",
                    ValidAudience            = jwtSection["Audience"] ?? "BlogManagementClient",
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew                = TimeSpan.Zero,
                    NameClaimType            = "UserId",
                    RoleClaimType            = "Role"
                };
            });

        // ── Authorization Policies ─────────────────────────────────────────
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("Role", "Admin"));

            options.AddPolicy("AuthorOnly", policy =>
                policy.RequireClaim("Role", "Author"));

            // Used by unified endpoints that accept both Admin and Author tokens.
            // Fine-grained permission checks (e.g. own-blog-only) are enforced
            // inside the handler using the JWT claims.
            options.AddPolicy("AdminOrAuthor", policy =>
                policy.RequireClaim("Role", "Admin", "Author"));
        });

        // ── Swagger / OpenAPI ──────────────────────────────────────────────
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "Blog Management API",
                Version     = "v1",
                Description = "REST API for the Blog Management System."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Enter your JWT token."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // ── Validators ────────────────────────────────────────────────────
        services.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();
        services.AddValidatorsFromAssemblyContaining<UserLoginValidator>();
        services.AddValidatorsFromAssemblyContaining<ForgotPasswordValidator>();
        services.AddValidatorsFromAssemblyContaining<ResetPasswordValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateCategoryValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateCategoryValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateBlogValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateBlogValidator>();

        // ── Repositories ──────────────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();

        // ── Utilities ─────────────────────────────────────────────────────
        services.AddSingleton<JwtHelper>();
        services.AddScoped<IEmailService, EmailService>();

        // ── Services ──────────────────────────────────────────────────────
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBlogService, BlogService>();

        return services;
    }
}