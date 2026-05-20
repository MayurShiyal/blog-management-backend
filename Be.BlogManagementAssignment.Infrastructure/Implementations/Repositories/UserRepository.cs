using Be.BlogManagementAssignment.Application.Interfaces.Repositories;
using Be.BlogManagementAssignment.Domain.Entities;
using Be.BlogManagementAssignment.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Be.BlogManagementAssignment.Infrastructure.Implementations.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRepository"/>.
/// Handles all user roles (Admin, Author, Visitor) in a single repository.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    /// <inheritdoc />
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    /// <inheritdoc />
    public Task<User?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
        => _context.Users
            .FirstOrDefaultAsync(u => u.VerificationToken == token, cancellationToken);

    /// <inheritdoc />
    public Task<User?> GetByResetPasswordTokenAsync(string token, CancellationToken cancellationToken = default)
        => _context.Users
            .FirstOrDefaultAsync(u => u.ResetPasswordToken == token, cancellationToken);

    /// <inheritdoc />
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
