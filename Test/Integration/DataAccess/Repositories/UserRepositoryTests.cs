using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;

namespace Test.Integration.DataAccess.Repositories;

[Trait("Category", "Integration")]
public class UserRepositoryTests
{
    [Fact]
    public async Task GetByUsernameAsync_ExistingUsername_ReturnsEntity()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        
        //Act
        var returnedEntity = await userRepository.GetByUsernameAsync(userEntity.Username);
        
        //Assert
        returnedEntity.Should().NotBeNull();
        returnedEntity.Should().BeEquivalentTo(userEntity);
    }
    
    [Fact]
    public async Task GetByUsernameAsync_InexistentUsername_ReturnsNull()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Act
        var returnedEntity = await userRepository.GetByUsernameAsync("test");
        
        //Assert
        returnedEntity.Should().BeNull();
    }
    
    [Fact]
    public async Task UsernameExists_ExistingUsername_ReturnsTrue()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        
        //Act
        var returnedBool = await userRepository.UsernameExistsAsync(userEntity.Username);
        
        //Assert
        returnedBool.Should().BeTrue();
    }
    
    [Fact]
    public async Task UsernameExists_InexistentUsername_ReturnsFalse()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Act
        var returnedEntity = await userRepository.UsernameExistsAsync("test");
        
        //Assert
        returnedEntity.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_InexistentUsername_ReturnsEntityAndUpdatesParam()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        userEntity.Id = Guid.Empty;
        
        //Act
        var addedEntity = await userRepository.AddAsync(userEntity);
        var entityInDb = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == addedEntity.Id);
        
        //Assert
        entityInDb.Should().NotBeNull();
        addedEntity.Should().NotBeNull();
        addedEntity.Should().BeEquivalentTo(entityInDb);
        addedEntity.Should().BeEquivalentTo(userEntity);
    }

    [Fact]
    public async Task AddAsync_ExistingUsername_Throws()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        
        //Act
        var act = () => userRepository.AddAsync(userEntity);
        
        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task GetSignatureSaltByUsernameAsync_ExistingUsername_ReturnsCorrectString()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        
        //Act
        var returnedString = await userRepository.GetSignatureSaltByUsernameAsync(userEntity.Username);
        
        //Assert
        returnedString.Should().NotBeNull();
        returnedString.Should().BeEquivalentTo(userEntity.SignatureSaltBase64);
    }
    
    [Fact]
    public async Task GetSignatureSaltByUsernameAsync_InexistentUsername_ReturnsNull()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Act
        var returnedString = await userRepository.GetSignatureSaltByUsernameAsync("test");
        
        //Assert
        returnedString.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteByIdAsync_ExistingId_DeletesCascadeReturnsTrue()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var userId = userEntity.Id;
        
        var noteEntity = TestDataProvider.GetNoteEntity(userId);
        await context.Notes.AddAsync(noteEntity);
        await context.SaveChangesAsync();
        
        //Act
        var notesForUserInDbBeforeDelete = await context.Notes.AsNoTracking().Where(n => n.UserId == userId).ToListAsync();
        var removed = await userRepository.DeleteByIdAsync(userId);
        var userEntityInDb = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        var notesForUserInDbAfterDelete = await context.Notes.AsNoTracking().Where(n => n.UserId == userId).ToListAsync();
        
        //Assert
        removed.Should().BeTrue();
        userEntityInDb.Should().BeNull();
        notesForUserInDbBeforeDelete.Should().NotBeEmpty();
        notesForUserInDbAfterDelete.Should().BeEmpty();
    }
    
    [Fact]
    public async Task DeleteByIdAsync_InexistentId_ReturnsFalse()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var userRepository = new  UserRepository(context);
        
        //Act
        var removed = await userRepository.DeleteByIdAsync(Guid.NewGuid());
        
        //Assert
        removed.Should().BeFalse();
    }
}