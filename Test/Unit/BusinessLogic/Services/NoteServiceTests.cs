using AutoMapper;
using BusinessLogic.Mapping;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Core.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Dto;
using Shared.Enums;
using Test.TestHelpers;

namespace Test.Unit.BusinessLogic.Services;

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
    public async Task GetAllForCurrentUser_ListNotEmptyAuth_ReturnsSuccessOK()
    {
        //Arrange
        var currentUserId = Guid.NewGuid().ToString();

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(currentUserId);
        _mockNoteRepository.Setup(n => n.GetAllByUserIdAsNoTrackingAsync(It.IsAny<Guid>()))
            .ReturnsAsync([TestDataProvider.GetNoteEntity(Guid.Parse(currentUserId))]);

        //Act
        var serviceResult = await _noteService.GetAllForCurrentUser();

        //Assert
        CommonAssertions.AssertServiceResultSuccess(serviceResult, ServiceResultSuccessType.Ok);
    }

    [Fact]
    public async Task GetAllForCurrentUser_ListEmptyAuth_ReturnsFailureNotFound()
    {
        //Arrange
        var currentUserId = Guid.NewGuid().ToString();

        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(currentUserId);
        _mockNoteRepository.Setup(n => n.GetAllByUserIdAsNoTrackingAsync(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        //Act
        var serviceResult = await _noteService.GetAllForCurrentUser();

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllForCurrentUser_NotAuth_ReturnsFailureUnauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
        var serviceResult = await _noteService.GetAllForCurrentUser();

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task AddAsync_ValidNoteAuth_ReturnsSuccessCreated()
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
    public async Task AddAsync_NotAuth_ReturnsFailureUnauthorized()
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
}