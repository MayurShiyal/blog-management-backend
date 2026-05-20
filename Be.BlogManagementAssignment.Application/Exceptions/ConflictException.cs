namespace Be.BlogManagementAssignment.Application.Exceptions;

/// <summary>
/// Thrown when a resource with the same unique key already exists.
/// Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, statusCode: 409)
    {
    }
}
