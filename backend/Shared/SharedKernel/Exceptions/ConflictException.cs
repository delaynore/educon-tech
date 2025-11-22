namespace SharedKernel.Exceptions;

public sealed class ConflictException : Exception
{
    public Error Error { get; set; }

    public ConflictException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
