using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.Exceptions;

public sealed class ValidationException : Exception
{
    public Error Error { get; set; }

    public ValidationException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public ValidationException()
    {
    }

    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
