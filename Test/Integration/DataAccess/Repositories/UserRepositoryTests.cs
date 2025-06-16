using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;

namespace Test.Integration.DataAccess.Repositories;

[Trait("Category", "Integration")]
public class UserRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    private readonly AppDbContext _dbContext;

    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _dbContext = new AppDbContext(_options);
        _userRepository = new UserRepository(_dbContext);
    }

    [Fact]
    public async Task GetByUsernameAsync_ExistingUsername_ReturnsEntity()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var returnedEntity = await _userRepository.GetByUsernameAsync(userEntity.Username);
        
        //Assert
        returnedEntity.Should().NotBeNull();
        returnedEntity.Should().BeEquivalentTo(userEntity);
    }
    
    [Fact]
    public async Task GetByUsernameAsync_InexistentUsername_ReturnsNull()
    {
        //Act
        var returnedEntity = await _userRepository.GetByUsernameAsync("test");
        
        //Assert
        returnedEntity.Should().BeNull();
    }
    
    [Fact]
    public async Task UsernameExists_ExistingUsername_ReturnsTrue()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var returnedBool = await _userRepository.UsernameExistsAsync(userEntity.Username);
        
        //Assert
        returnedBool.Should().BeTrue();
    }
    
    [Fact]
    public async Task UsernameExists_InexistentUsername_ReturnsFalse()
    {
        //Act
        var returnedEntity = await _userRepository.UsernameExistsAsync("test");
        
        //Assert
        returnedEntity.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_InexistentUsername_ReturnsEntityAndUpdatesParam()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        userEntity.Id = Guid.Empty;
        
        //Act
        var addedEntity = await _userRepository.AddAsync(userEntity);
        var entityInDb = await _dbContext.Users.FindAsync(addedEntity.Id);
        
        //Assert
        entityInDb.Should().NotBeNull();
        addedEntity.Should().NotBeNull();
        addedEntity.Should().BeEquivalentTo(entityInDb);
        addedEntity.Should().BeEquivalentTo(userEntity);
    }

    [Fact]
    public async Task AddAsync_ExistingUsername_Throws()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var act = () => _userRepository.AddAsync(userEntity);
        
        //Assert
        //InMemoryDatabase throws ArgumentException not DatabaseUpdateException
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    [Fact]
    public async Task GetSignatureSaltByUsernameAsync_ExistingUsername_ReturnsCorrectString()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var returnedString = await _userRepository.GetSignatureSaltByUsernameAsync(userEntity.Username);
        
        //Assert
        returnedString.Should().NotBeNull();
        returnedString.Should().BeEquivalentTo(userEntity.SignatureSaltBase64);
    }
    
    [Fact]
    public async Task GetSignatureSaltByUsernameAsync_InexistentUsername_ReturnsNull()
    {
        //Act
        var returnedString = await _userRepository.GetSignatureSaltByUsernameAsync("test");
        
        //Assert
        returnedString.Should().BeNull();
    }
}