namespace EducationContentService.Domain.Shared;

public sealed record ErrorMessage(string Code, string Message, string? InvalidField = null);

public sealed record Error
{
    private Error(IEnumerable<ErrorMessage> messages, ErrorType type)
    {
        Messages = [.. messages];
        Type = type;
    }

    public IReadOnlyList<ErrorMessage> Messages { get; }

    public ErrorType Type { get; }

    public static Error Validation(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.Validation);
    }

    public static Error NotFound(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.NotFound);
    }

    public static Error Failure(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.Failure);
    }

    public static Error Conflict(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.Conflict);
    }

    public static Error Authentication(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.Authentication);
    }

    public static Error Authorization(params IEnumerable<ErrorMessage> messages)
    {
        return new Error(messages, ErrorType.Authorization);
    }
}
