using EducationContentService.Domain.Shared;

namespace EducationContentService.Domain.Exceptions;

public sealed class NotFoundException : Exception
{
    public Error Error { get; set; }

    public NotFoundException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
