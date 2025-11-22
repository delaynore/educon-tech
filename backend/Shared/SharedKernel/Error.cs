using System.Text.Json.Serialization;

namespace SharedKernel;

public sealed record ErrorMessage(string Code, string Message, string? InvalidField = null);

public sealed record Error
{
    private Error(IEnumerable<ErrorMessage> messages, ErrorType type)
    {
        Messages = [.. messages];
        Type = type;
    }

    [JsonConstructor]
    private Error(IReadOnlyList<ErrorMessage> messages, ErrorType type)
    {
        Messages = [.. messages];
        Type = type;
    }

    public IReadOnlyList<ErrorMessage> Messages { get; }

    public ErrorType Type { get; }

    public string GetMessage() => string.Join(';', Messages);

    public static Error Validation(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.Validation);

    public static Error NotFound(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.NotFound);

    public static Error Failure(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.Failure);

    public static Error Conflict(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.Conflict);

    public static Error Authentication(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.Authentication);

    public static Error Authorization(string code, string message, string? invalidField = null)
        => new([new(code, message, invalidField)], ErrorType.Authorization);

    public static Error Validation(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.Validation);

    public static Error NotFound(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.NotFound);

    public static Error Failure(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.Failure);

    public static Error Conflict(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.Conflict);

    public static Error Authentication(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.Authentication);

    public static Error Authorization(params IEnumerable<ErrorMessage> messages)
        => new(messages, ErrorType.Authorization);
}
