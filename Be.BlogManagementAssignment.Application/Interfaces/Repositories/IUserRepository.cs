using Be.BlogManagementAssignment.Domain.Entities;

namespace Be.BlogManagementAssignment.Application.Interfaces.Repositories;

public interface IUserRepository
{
    /// <summary>Returns true when any user record with the given email exists (any role).</summary>
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Persists a new user and returns the saved entity.</summary>
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Returns the user with the given email, or null if not found.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Returns the user whose VerificationToken matches, or null if not found.</summary>
    Task<User?> GetByVerificationTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Returns the user whose ResetPasswordToken matches, or null if not found.</summary>
    Task<User?> GetByResetPasswordTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Persists changes to an existing user entity.</summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
