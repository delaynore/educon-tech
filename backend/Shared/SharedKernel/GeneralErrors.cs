namespace SharedKernel;

public static class GeneralErrors
{
    public static Error ValueIsInvalid(string? name = null)
    {
        var label = name ?? "value";

        return Error.Validation("value.invalid", $"{label} is invalid", name);
    }

    public static Error NotFound(Guid? id = null, string? name = null)
    {
        var forId = id is null ? string.Empty : $" by {id}";
        return Error.NotFound("record.not.found", $"{name ?? "record"} is not found{forId}", name);
    }

    public static Error ValueIsRequired(string? name = null)
    {
        return Error.Validation("value.required", $"{name ?? "value"} is required", name);
    }

    public static Error AlreadyExist(string? name = null)
    {
        return Error.Conflict("record.already.exist", "record already exists", name);
    }

    public static Error Failure(string? message = null)
    {
        return Error.Failure("server.failure", message ?? "server failure");
    }
}
