using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;

namespace Shared.Validations;

public class ValidBase64Attribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string stringValue ||
            !Convert.TryFromBase64String(stringValue, new byte[((stringValue.Length + 3) / 4) * 3], out _))
            return new ValidationResult($"{validationContext.DisplayName} must be a valid base64 string.");

        return ValidationResult.Success;
    }
}