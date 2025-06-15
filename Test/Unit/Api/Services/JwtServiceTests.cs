using System.Security.Claims;
using EncryptedNotes.Configurations;
using EncryptedNotes.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Test.Unit.Api.Services;

public class JwtServiceTests
{
    private const string PrivateKey = "secret-test-key-at-least-128-bits-long";
    private const int LifetimeInMinutes = 60;
    private const string Audience = "test-audience";
    private const string Issuer = "test-issuer";
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        //Mock configuration for IOptions<JwtSettings>
        var jwtSettings = Options.Create(new JwtSettings
        {
            PrivateKey = PrivateKey,
            LifetimeInMinutes = LifetimeInMinutes,
            Audience = Audience,
            Issuer = Issuer,
        });

        _jwtService = new JwtService(jwtSettings);
    }

    [Fact]
    public void GenerateToken_TokenContainsAllCorrectClaims()
    {
        //Arrange
        const string username = "test-username";
        var userId = Guid.NewGuid();

        //Act
        var token = _jwtService.GenerateToken(username, userId);
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(token);
        
        //Getting iat and exp as time to compare them
        var iatTime = DateTimeOffset.FromUnixTimeSeconds(jwt.GetPayloadValue<long>("iat"));
        var expTime = DateTimeOffset.FromUnixTimeSeconds(jwt.GetPayloadValue<long>("exp"));

        //Assert
        jwt.GetPayloadValue<string>(ClaimTypes.NameIdentifier)
            .Should().Be(userId.ToString(), "because the 'name identifier' claim should match the one passed to GenerateToken");
        
        jwt.GetPayloadValue<string>(ClaimTypes.Name)
            .Should().Be(username, "because the 'name' claim should match the one passed to GenerateToken");

        jwt.GetPayloadValue<string>("aud")
            .Should().Be(Audience, "because the 'audience' claim should match the JwtSettings Audience");

        jwt.GetPayloadValue<string>("iss")
            .Should().Be(Issuer, "because the 'issuer' claim should match the JwtSettings Issuer");

        jwt.GetPayloadValue<string>("nbf")
            .Should().Be(jwt.GetPayloadValue<string>("iat"), "because the 'not before' claim should be the same as 'issued at'");

        (expTime - iatTime).TotalMinutes
            .Should().Be(LifetimeInMinutes, "because the expiration time should be 'LifetimeInMinutes' minutes after the 'issued at' time");
    }
}