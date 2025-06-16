using DataAccess;
using DataAccess.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;

namespace Test.Integration.DataAccess.Repositories;

public class NoteRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    private readonly AppDbContext _dbContext;

    private readonly NoteRepository _noteRepository;

    public NoteRepositoryTests()
    {
        _dbContext = new AppDbContext(_options);
        _noteRepository = new NoteRepository(_dbContext);
    }
    
    [Fact]
    public async Task GetAllByUserIdAsync_ExistingId_ReturnsEntityList()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        var noteEntity = TestDataProvider.GetNoteEntity(userEntity.Id);
        await _dbContext.Notes.AddAsync(noteEntity);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var returnedEntityList = await _noteRepository.GetAllByUserIdAsync(userEntity.Id);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Count.Should().Be(1);
        returnedEntityList[0].Should().BeEquivalentTo(noteEntity, options => options.Excluding(n => n.User));
    }
    
    [Fact]
    public async Task GetAllByUserIdAsync_InexistentId_ReturnsEmptyList()
    {
        //Act
        var returnedEntityList = await _noteRepository.GetAllByUserIdAsync(Guid.NewGuid());
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetPageByUserIdAsync_ExistingId_ReturnsEntityListAndCorrectCount()
    {
        //Arrange
        var userEntity = TestDataProvider.GetUserEntity();
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();
        var noteEntity1 = TestDataProvider.GetNoteEntity(userEntity.Id);
        var noteEntity2 = TestDataProvider.GetNoteEntity(userEntity.Id);
        await _dbContext.Notes.AddAsync(noteEntity1);
        await _dbContext.Notes.AddAsync(noteEntity2);
        await _dbContext.SaveChangesAsync();
        
        //Act
        var (returnedEntityList, returnedTotalCount) = await _noteRepository.GetPageByUserIdAsync(userEntity.Id, 1, 1);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Count.Should().Be(1);
        returnedTotalCount.Should().Be(2);
        returnedEntityList[0].Should().BeEquivalentTo(noteEntity2, options => options.Excluding(n => n.User));
    }
    
    [Fact]
    public async Task GetPageByUserIdAsync_InexistentId_ReturnsEmptyListAndCorrectCount()
    {
        //Act
        var (returnedEntityList, returnedTotalCount) = await _noteRepository.GetPageByUserIdAsync(Guid.NewGuid(), 1, 1);
        
        //Assert
        returnedEntityList.Should().NotBeNull();
        returnedEntityList.Should().BeEmpty();
        returnedTotalCount.Should().Be(0);
    }
    
    [Fact]
    public async Task AddAsync_InexistentUsername_ReturnsEntityAndUpdatesParam()
    {
        //Arrange
        var noteEntity = TestDataProvider.GetNoteEntity(Guid.NewGuid());
        noteEntity.Id = Guid.Empty;
        
        //Act
        var addedEntity = await _noteRepository.AddAsync(noteEntity);
        var entityInDb = await _dbContext.Notes.FindAsync(addedEntity.Id);
        
        //Assert
        entityInDb.Should().NotBeNull();
        addedEntity.Should().NotBeNull();
        addedEntity.Should().BeEquivalentTo(entityInDb);
        addedEntity.Should().BeEquivalentTo(noteEntity);
    }
}