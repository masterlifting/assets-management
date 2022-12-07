using System.ComponentModel.DataAnnotations;

namespace Shared.Attributes.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LowerAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string? oldValue = null;

        if (value is string)
        {
            oldValue = value.ToString();
            value = oldValue?.ToLowerInvariant();
        }
        else
            ErrorMessage = $"The '{value}' must be string!";

        return oldValue is not null
               && value is string newValue
               && oldValue.Equals(newValue, StringComparison.InvariantCulture);
    }
}