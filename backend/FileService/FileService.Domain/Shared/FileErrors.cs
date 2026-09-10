using SharedKernel;

namespace FileService.Domain.Shared;

public static class FileErrors
{
    public static Error BucketNotFound()
    {
        return Error.NotFound("no.such.bucket", "Бакет не найден");
    }

    public static Error UploadNotFound()
    {
        return Error.NotFound(
            "upload.not.found",
            $"Сессия загрузки не найдена");
    }

    public static Error ObjectNotFound(string? objectKey = null)
    {
        var key = objectKey is null ? string.Empty : $"с ключом {objectKey} ";

        return Error.NotFound("object.not.found", $"Объект {key}не найден");
    }

    public static Error Forbidden()
    {
        return Error.Failure("access.denied", "Недостаточно прав для выполнения операции");
    }

    public static Error ValidationFailed()
    {
        var message = "Запрос содержит некорректные данные";

        return Error.Validation("validation.failed", message);
    }

    public static Error InternalServerError()
    {
        return Error.Failure("internal.server.error", "Внутренняя ошибка хранилища");
    }

    public static Error OperationCanceled()
    {
        return Error.Failure("operation.canceled", "Операция была отменена");
    }

    public static Error NetworkIssue()
    {
        return Error.Failure(
            "network.issue",
            "Сетевая ошибка при взаимодействии с файловым хринилищем");
    }

    public static Error Unknown()
    {
        return Error.Failure("unknown.error", "Произошла неизвестная ошибка");
    }

    public static Error DatabaseError()
    {
        return Error.Failure(
            "files.database.error",
            "Ошибка базы данных при работе с сервисом - files");
    }

    public static Error OperationCancelled()
    {
        return Error.Failure(
            "files.operation.cancelled",
            "Операция была отменена");
    }
}
