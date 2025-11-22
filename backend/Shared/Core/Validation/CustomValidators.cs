using System.Text.Json;
using CSharpFunctionalExtensions;
using FluentValidation;
using SharedKernel;

namespace Core.Validation;

public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TProperty> MustBeValueObject<T, TProperty, TValueObject>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        Func<TProperty, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod.Invoke(value);

            if (result.IsSuccess)
            {
                return;
            }

            context.AddFailure(JsonSerializer.Serialize(result.Error));
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        Error error)
    {
        return rule.WithMessage(JsonSerializer.Serialize(error));
    }
}
