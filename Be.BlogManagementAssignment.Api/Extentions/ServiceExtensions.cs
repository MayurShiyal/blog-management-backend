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
        var secretKey = jwtSection["SecretKey"]
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

                    ValidIssuer   = jwtSection["Issuer"]   ?? "BlogManagementApi",
                    ValidAudience = jwtSection["Audience"] ?? "BlogManagementClient",

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),

                    ClockSkew = TimeSpan.FromMinutes(5),

                    // FIX: Map custom claim names so the ClaimsPrincipal is populated
                    // correctly.  NameClaimType drives HttpContext.User.Identity.Name;
                    // RoleClaimType drives User.IsInRole() and [Authorize(Roles=…)].
                    // Both must match the exact claim keys emitted by JwtHelper.
                    NameClaimType = "UserId",
                    RoleClaimType = "Role"
                };

                // FIX: Add comprehensive JWT event logging so token failures are
                // visible in the console/log output rather than silently returning 401.
                options.Events = new JwtBearerEvents
                {
                    // Fires when the token cannot be validated (expired, bad signature,
                    // wrong issuer/audience, etc.).  This is the most useful event for
                    // diagnosing 401 responses.
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogError(
                            context.Exception,
                            "JWT authentication failed for {Path} — {ExceptionType}: {Message}",
                            context.Request.Path,
                            context.Exception.GetType().Name,
                            context.Exception.Message);

                        return Task.CompletedTask;
                    },

                    // Fires when a 401 challenge is about to be sent (no token supplied
                    // or token validation failed).  Logs the exact failure reason.
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogWarning(
                            "JWT challenge on {Path} — Error: {Error}, Description: {Description}",
                            context.Request.Path,
                            context.Error          ?? "(none)",
                            context.ErrorDescription ?? "(none)");

                        return Task.CompletedTask;
                    },

                    // Fires when the user is authenticated but lacks the required
                    // policy/role (403 Forbidden).
                    OnForbidden = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogWarning(
                            "JWT forbidden on {Path} — user is authenticated but does not satisfy the policy.",
                            context.Request.Path);

                        return Task.CompletedTask;
                    },

                    // Fires when a token is successfully validated.  Useful to confirm
                    // that the principal and its claims look correct.
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        var userId = context.Principal?.FindFirst("UserId")?.Value ?? "(unknown)";
                        var role   = context.Principal?.FindFirst("Role")?.Value   ?? "(unknown)";

                        logger.LogDebug(
                            "JWT validated for UserId={UserId} Role={Role} on {Path}",
                            userId, role, context.Request.Path);

                        return Task.CompletedTask;
                    }
                };
            });

        // ── Authorization Policies ─────────────────────────────────────────
        services.AddAuthorization(options =>
        {
            // FIX: Use ClaimTypes-consistent check.  Because RoleClaimType is mapped to
            // "Role" above, User.IsInRole("Admin") also works — but RequireClaim is
            // explicit and avoids any mapping ambiguity.
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

            // FIX: SecurityScheme Id must be "Bearer" (capital B) and the Reference Id
            // in the SecurityRequirement must match exactly — otherwise Swagger's
            // Authorize button sends the token but the server ignores it.
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name        = "Authorization",
                Type        = SecuritySchemeType.Http,
                Scheme      = "Bearer",
                BearerFormat = "JWT",
                In          = ParameterLocation.Header,
                Description = "Enter your JWT token (without the 'Bearer ' prefix — Swagger adds it automatically)."
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