using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;

namespace Test.Integration.DataAccess.Repositories;

[Trait("Category", "Integration")]
public class NoteRepositoryTests
{
    [Fact]
    public async Task GetAllByUserIdAsync_ExistingId_ReturnsEntityList()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var noteEntity = TestDataProvider.GetNoteEntity(userEntity.Id);
        await context.Notes.AddAsync(noteEntity);
        await context.SaveChangesAsync();
        
        //Act
        var returnedEntityList = await noteRepository.GetAllByUserIdAsync(userEntity.Id);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Count.Should().Be(1);
        returnedEntityList[0].Should().BeEquivalentTo(noteEntity, options => options.Excluding(n => n.User));
    }
    
    [Fact]
    public async Task GetAllByUserIdAsync_InexistentId_ReturnsEmptyList()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Act
        var returnedEntityList = await noteRepository.GetAllByUserIdAsync(Guid.NewGuid());
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetPageByUserIdAsync_ExistingId_ReturnsEntityListAndCorrectCount()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var noteEntity1 = TestDataProvider.GetNoteEntity(userEntity.Id);
        var noteEntity2 = TestDataProvider.GetNoteEntity(userEntity.Id);
        await context.Notes.AddRangeAsync(noteEntity1, noteEntity2);
        await context.SaveChangesAsync();
        
        //Act
        var (returnedEntityList, returnedTotalCount) = await noteRepository.GetPageByUserIdAsync(userEntity.Id, 1, 1);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Count.Should().Be(1);
        returnedTotalCount.Should().Be(2);
        returnedEntityList[0].Should().BeEquivalentTo(noteEntity2, options => options.Excluding(n => n.User));
    }
    
    [Fact]
    public async Task GetPageByUserIdAsync_InexistentId_ReturnsEmptyListAndCorrectCount()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Act
        var (returnedEntityList, returnedTotalCount) = await noteRepository.GetPageByUserIdAsync(Guid.NewGuid(), 1, 1);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Should().BeEmpty();
        returnedTotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task AddAsync_ValidAdd_ReturnsEntityAndUpdatesParam()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var noteEntity = TestDataProvider.GetNoteEntity(userEntity.Id);
        noteEntity.Id = Guid.Empty;
        
        //Act
        var addedEntity = await noteRepository.AddAsync(noteEntity);
        var entityInDb = await context.Notes.FindAsync(addedEntity.Id);
        
        //Assert
        entityInDb.Should().NotBeNull();
        addedEntity.Should().NotBeNull();
        addedEntity.Should().BeEquivalentTo(entityInDb);
        addedEntity.Should().BeEquivalentTo(noteEntity);
    }
    
    [Fact]
    public async Task AddAsync_InexistentUserId_Throws()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var noteEntity = TestDataProvider.GetNoteEntity(Guid.NewGuid());
        noteEntity.Id = Guid.Empty;
        
        //Act
        var act = () => noteRepository.AddAsync(noteEntity);
        
        //Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
    
    [Fact]
    public async Task DeleteByIdAsync_ExistingId_DeletesReturnsTrue()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        
        var noteEntity = TestDataProvider.GetNoteEntity(userEntity.Id);
        await context.Notes.AddAsync(noteEntity);
        await context.SaveChangesAsync();
        var notedId = noteEntity.Id;
        
        //Act
        var removed = await noteRepository.DeleteByIdAsync(notedId);
        var entityInDb = await context.Notes.Where(n => n.Id == notedId).SingleOrDefaultAsync();
        
        //Assert
        removed.Should().BeTrue();
        entityInDb.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteByIdAsync_InexistentId_ReturnsFalse()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Act
        var removed = await noteRepository.DeleteByIdAsync(Guid.NewGuid());
        
        //Assert
        removed.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteByIdAndUserIdAsync_ExistingIdCorrectUserId_DeletesReturnsTrue()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var userId = userEntity.Id;
        
        var noteEntity = TestDataProvider.GetNoteEntity(userId);
        await context.Notes.AddAsync(noteEntity);
        await context.SaveChangesAsync();
        var notedId = noteEntity.Id;
        
        //Act
        var removed = await noteRepository.DeleteByIdAndUserIdAsync(notedId, userId);
        var entityInDb = await context.Notes.Where(n => n.Id == notedId).SingleOrDefaultAsync();
        
        //Assert
        removed.Should().BeTrue();
        entityInDb.Should().BeNull();
    }
    
    [Fact]
    public async Task DeleteByIdAndUserIdAsync_InexistentId_ReturnsFalse()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Act
        var removed = await noteRepository.DeleteByIdAndUserIdAsync(Guid.NewGuid(), Guid.NewGuid());
        
        //Assert
        removed.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteByIdAndUserIdAsync_ExistingIdWrongUserId_DoesntDeleteReturnsFalse()
    {
        using var provider = new SqLiteInMemoryDbContextProvider<AppDbContext>();
        var context = provider.Context;
        var noteRepository = new  NoteRepository(context);
        
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
        var userId = userEntity.Id;
        
        var noteEntity = TestDataProvider.GetNoteEntity(userEntity.Id);
        await context.Notes.AddAsync(noteEntity);
        await context.SaveChangesAsync();
        var notedId = noteEntity.Id;
        
        //Act
        var removed = await noteRepository.DeleteByIdAndUserIdAsync(notedId, Guid.NewGuid());
        var entityInDb = await context.Notes.Where(n => n.Id == notedId).SingleOrDefaultAsync();
        
        //Assert
        removed.Should().BeFalse();
        entityInDb.Should().NotBeNull();
    }
}