using AutoMapper;
using BusinessLogic.Mapping;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto.Requests;
using Shared.Enums;
using Test.TestHelpers;

namespace Test.Unit.BusinessLogic.Services;

[Trait("Category", "Unit")]
public class NoteServiceTests
{
    private readonly Mock<INoteRepository> _mockNoteRepository = new();
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<ILogger<NoteService>> _mockLogger = new();

    private readonly IMapper _realMapper = new Mapper(new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<BusinessLogicMappingProfile>();
    }));

    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        _noteService = new NoteService(_mockNoteRepository.Object, _mockCurrentUserService.Object, _realMapper,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllForCurrentUser_Auth_ReturnsSuccessOK()
    {
        //Arrange
        var currentUserId = Guid.NewGuid().ToString();

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(currentUserId);
        _mockNoteRepository.Setup(n => n.GetAllByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync([TestDataProvider.GetNoteEntity(Guid.Parse(currentUserId))]);

        //Act
#pragma warning disable CS0618 // Suppress 'obsolete' warning
        var serviceResult = await _noteService.GetAllForCurrentUser();
#pragma warning restore CS0618

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task GetAllForCurrentUser_NotAuth_ReturnsFailureUnauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
#pragma warning disable CS0618 // Suppress 'obsolete' warning
        var serviceResult = await _noteService.GetAllForCurrentUser();
#pragma warning restore CS0618

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task GetPageForCurrentUser_Auth_ReturnsSuccessOK()
    {
        //Arrange
        var currentUserId = Guid.NewGuid().ToString();
        var paginatedNotesRequest = new PaginatedNotesRequest()
        {
            Page = 1,
            PageSize = 10,
        };

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(currentUserId);
        _mockNoteRepository.Setup(n => n.GetPageByUserIdAsync(Guid.Parse(currentUserId),
                paginatedNotesRequest.Page,
                paginatedNotesRequest.PageSize))
            .ReturnsAsync(([TestDataProvider.GetNoteEntity(Guid.Parse(currentUserId))], 1));

        //Act
        var serviceResult = await _noteService.GetPageForCurrentUser(paginatedNotesRequest);

        //Assert
        serviceResult.Data.Should().NotBeNull();
        serviceResult.Data.Page.Should().Be(paginatedNotesRequest.Page);
        serviceResult.Data.PageSize.Should().Be(paginatedNotesRequest.PageSize);
        serviceResult.Data.Items.Count.Should().Be(1);
        serviceResult.Data.HasMore.Should().BeFalse();
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task GetPageForCurrentUser_NotAuth_ReturnsFailureUnauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);
        var paginatedNotesRequest = new PaginatedNotesRequest()
        {
            Page = 1,
            PageSize = 10,
        };

        //Act
        var serviceResult = await _noteService.GetPageForCurrentUser(paginatedNotesRequest);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task AddAsyncToCurrentUser_ValidNoteAuth_ReturnsSuccessCreated()
    {
        //Arrange
        var currentUserId = Guid.NewGuid();
        var noteDto = TestDataProvider.GetNoteDto(currentUserId);

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId)
            .Returns(currentUserId.ToString);
        _mockNoteRepository.Setup(n => n.AddAsync(It.IsAny<NoteEntity>())).Callback<NoteEntity>(e =>
        {
            e.Id = Guid.NewGuid();
        });

        //Act
        var serviceResult = await _noteService.AddAsyncToCurrentUser(noteDto);

        //Assert
        serviceResult.Data?.Id.Should().NotBeEmpty();
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Created);
    }

    [Fact]
    public async Task AddAsyncToCurrentUser_NotAuth_ReturnsFailureUnauthorized()
    {
        //Arrange
        var noteDto = TestDataProvider.GetNoteDto(Guid.NewGuid());

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId)
            .Returns(null as string);

        //Act
        var serviceResult = await _noteService.AddAsyncToCurrentUser(noteDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task DeleteByIdForCurrentUserAsync_AuthAndDeleted_ReturnsSuccessNoContent()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid().ToString);
        _mockNoteRepository.Setup(n => n.DeleteByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        //Act
        var serviceResult = await _noteService.DeleteByIdForCurrentUserAsync(Guid.NewGuid());

        //Assert
        CommonAssertions.AssertServiceResultSuccessNoContent(serviceResult);
    }

    [Fact]
    public async Task DeleteByIdForCurrentUserAsync_AuthAndNotDeleted_ReturnsFailureNotFound()
    {
        //This can happen if the user has been deleted and a new delete request is received with the old still valid token
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(Guid.NewGuid().ToString);
        _mockNoteRepository.Setup(n => n.DeleteByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false); //Mock repository not finding user in db and so not deleting anything

        //Act
        var serviceResult = await _noteService.DeleteByIdForCurrentUserAsync(Guid.NewGuid());

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteByIdForCurrentUserAsync_NotAuth_ReturnsFailureUnauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
        var serviceResult = await _noteService.DeleteByIdForCurrentUserAsync(Guid.NewGuid());

        //Assert
        _mockNoteRepository.Verify(n => n.DeleteByIdAndUserIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateForCurrentUserAsync_AuthAndUpdated_ReturnsSuccessOk()
    {
        //Arrange
        var userId =  Guid.NewGuid();
        var noteDto = TestDataProvider.GetNoteDto(userId);
        
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(userId.ToString);
        _mockNoteRepository.Setup(n => n.UpdateForUserIdAsync(It.IsAny<NoteEntity>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        //Act
        var serviceResult = await _noteService.UpdateForCurrentUserAsync(noteDto);

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }
    
    [Fact]
    public async Task UpdateForCurrentUserAsync_AuthAndNotUpdated_ReturnsFailureNotFound()
    {
        //Arrange
        var userId =  Guid.NewGuid();
        var noteDto = TestDataProvider.GetNoteDto(userId);
        
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(userId.ToString);
        _mockNoteRepository.Setup(n => n.UpdateForUserIdAsync(It.IsAny<NoteEntity>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        //Act
        var serviceResult = await _noteService.UpdateForCurrentUserAsync(noteDto);

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }
    
    [Fact]
    public async Task UpdateForCurrentUserAsync_NotAuth_ReturnsFailureUnauthorized()
    {
        //Arrange
        var noteDto = TestDataProvider.GetNoteDto(Guid.NewGuid());
        
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
        var serviceResult = await _noteService.UpdateForCurrentUserAsync(noteDto);

        //Assert
        _mockNoteRepository.Verify(n => n.UpdateForUserIdAsync(It.IsAny<NoteEntity>(), It.IsAny<Guid>()), Times.Never);
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }
}