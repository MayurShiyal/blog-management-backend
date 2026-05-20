using Be.BlogManagementAssignment.Api.Extentions;
using Be.BlogManagementAssignment.Infrastructure.Persistence;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

// ── Seed database ─────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedAdminAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog Management API v1");
        options.RoutePrefix = "swagger";
    });
}

// ── Remove UseHttpsRedirection — backend runs on HTTP (port 5121) ──────────
// app.UseHttpsRedirection();

// ── CORS — must be placed before auth middleware ───────────────────────────
app.UseCors("AllowFrontend");

// ── Authentication & Authorization middleware ──────────────────────────────
app.UseAuthentication();
app.UseAuthorization();

app.MapAllModules();
app.Run();