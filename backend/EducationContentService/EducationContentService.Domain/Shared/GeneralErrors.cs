namespace EducationContentService.Domain.Shared;

public static class GeneralErrors
{
    public static Error ValueIsInvalid(string? name = null)
    {
        var label = name ?? "value";

        return Error.Validation(new ErrorMessage("value.invalid", $"{label} is invalid"));
    }

    public static Error NotFound(Guid? id = null, string? name = null)
    {
        var forId = id is null ? string.Empty : $" by {id}";
        return Error.NotFound("record.not.found", $"{name ?? "record"} is not found{forId}");
    }

    public static Error ValueIsRequired(string? name = null)
    {
        return Error.Validation("value.required", $"{name ?? "value"} is required");
    }

    public static Error AlreadyExist()
    {
        return Error.Conflict("record.already.exist", "record already exists");
    }

    public static Error Failure(string? message = null)
    {
        return Error.Failure("server.failure", message ?? "server failure");
    }
}
