namespace Be.BlogManagementAssignment.Application.Exceptions;

/// <summary>
/// Base exception for all application-level, user-facing errors.
/// Carries an HTTP status code so the middleware can map it correctly.
/// </summary>
public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
