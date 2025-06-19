using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Shared.Validations;

namespace Test.Unit.Shared.Validations;

public class CustomStringLengthAttributeTests
{
    private const int MinLength = 2;
    private const int MaxLength = 5;
    private readonly CustomStringLengthAttribute _customStringLengthAttribute = new(MaxLength, MinLength);
    
    private readonly ValidationContext _validationContext = new (new{})
    {
        DisplayName = "test"
    };

    [Fact]
    public void IsValid_BetweenMaxAndMin_ReturnsSuccess()
    {
        //Arrange
        const string validString = "123";
        
        //Act
        var validationResult = _customStringLengthAttribute.GetValidationResult(validString, _validationContext);
        
        //Assert
        validationResult.Should().Be(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_TooShort_ReturnsFailure()
    {
        //Arrange
        const string shortString = "1";
        
        //Act
        var validationResult = _customStringLengthAttribute.GetValidationResult(shortString, _validationContext);

        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_TooLong_ReturnsFailure()
    {
        //Arrange
        const string longString = "123456";
        
        //Act
        var validationResult = _customStringLengthAttribute.GetValidationResult(longString, _validationContext);

        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_NotAString_ReturnsFailure()
    {
        //Arrange
        const int integer = 42;
        
        //Act
        var validationResult = _customStringLengthAttribute.GetValidationResult(integer, _validationContext);

        //Assert
        validationResult.Should().NotBe(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_Null_ReturnsSuccess()
    {
        //Act
        var validationResult = _customStringLengthAttribute.GetValidationResult(null, _validationContext);
        
        //Assert
        validationResult.Should().Be(ValidationResult.Success);
    }
    
    [Fact]
    public void IsValid_AttributeWithoutMin_EmptyString_ReturnsSuccess()
    {
        //Arrange
        CustomStringLengthAttribute customStringLengthAttribute = new(MaxLength);
    
        ValidationContext validationContext = new (new{})
        {
            DisplayName = "test"
        };
        var emptyString = string.Empty;
        
        //Act
        var validationResult = customStringLengthAttribute.GetValidationResult(emptyString, _validationContext);
        
        //Assert
        validationResult.Should().Be(ValidationResult.Success);
    }
    
    [Fact]
    public void Constuctor_MinGreaterThanMax_ThrowsArgumentException()
    {
        //Arrange
        var caughtArgumentException = false;
        
        //Act
        try
        {
            CustomStringLengthAttribute customStringLengthAttribute = new(minLength: 10, maxLength: 5);
        }
        catch (ArgumentException)
        {
            caughtArgumentException = true;
        }
        
        //Assert
        caughtArgumentException.Should().BeTrue();
    }
}