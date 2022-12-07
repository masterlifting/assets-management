using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes.Validation;

public sealed class MoreZeroAttribute : ValidationAttribute
{
    private readonly string _property;
    public MoreZeroAttribute(string property) => _property = property;

    public override bool IsValid(object? value)
    {
        var stringValue = value?.ToString();

        var isParse = decimal.TryParse(stringValue, out var result);

        ErrorMessage = $"The '{_property}' must be greater than 0";

        return isParse && result > 0;
    }
}