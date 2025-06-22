using System.ComponentModel.DataAnnotations;

namespace Shared.Validations;

public class CustomStringLengthAttribute : ValidationAttribute
{
    private readonly int _minLength;
    private readonly int _maxLength;

    public CustomStringLengthAttribute(int maxLength, int minLength = 0)
    {
        if (minLength > maxLength)
            throw new ArgumentException("minLength must be less than or equal to maxLength");

        _minLength = minLength;
        _maxLength = maxLength;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        if (value is not string stringValue)
            return new ValidationResult($"{validationContext.DisplayName} must be a string");


        if (stringValue.Length < _minLength || stringValue.Length > _maxLength)
            return new ValidationResult(
                $"{validationContext.DisplayName} must be between {_minLength} and {_maxLength} characters long");

        return ValidationResult.Success;
    }
}