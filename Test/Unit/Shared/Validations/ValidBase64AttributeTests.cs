using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Shared.Validations;
using Test.TestHelpers;

namespace Test.Unit.Shared.Validations;

public class ValidBase64AttributeTests
{
    private readonly ValidBase64Attribute _validBase64Attribute = new();
    
    private readonly ValidationContext _validationContext = new (new{})
    {
        DisplayName = "test"
    };

    [Fact]
    public void IsValid_ValidBase64_ReturnsSuccess()
    {
        //Arrange
        var validBase64 = TestDataProvider.GetValidBase64Value();
        
        //Act
        var validationResult = _validBase64Attribute.GetValidationResult(validBase64, _validationContext);
        
        //Assert
        validationResult.Should().Be(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_InvalidBase64_ReturnsFailure()
    {
        //Arrange
        var invalidBase64 = TestDataProvider.GetInvalidBase64Value();
        
        //Act
        var validationResult = _validBase64Attribute.GetValidationResult(invalidBase64, _validationContext);
        
        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_NotAString_ReturnsFailure()
    {
        //Arrange
        const int integer = 1;
        
        //Act
        var validationResult = _validBase64Attribute.GetValidationResult(integer, _validationContext);
        
        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_Null_ReturnsFailure()
    {
        //Act
        var validationResult = _validBase64Attribute.GetValidationResult(null, _validationContext);
        
        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
}