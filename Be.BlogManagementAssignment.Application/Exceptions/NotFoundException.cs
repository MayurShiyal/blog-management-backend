namespace Be.BlogManagementAssignment.Application.Exceptions;

/// <summary>
/// Thrown when a requested resource cannot be found.
/// Maps to HTTP 404 Not Found.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, statusCode: 404)
    {
    }
}
