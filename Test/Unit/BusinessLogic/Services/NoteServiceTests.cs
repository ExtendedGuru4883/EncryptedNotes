using AutoMapper;
using BusinessLogic.Mapping;
using BusinessLogic.Services;
using Core.Abstractions.DataAccess.Repositories;
using Core.Abstractions.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
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
    public async Task GetAllForCurrentUser_CurrentUserIdNotNullListNotEmpty_Returns200OK()
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
    public async Task GetAllForCurrentUser_CurrentUserIdNotNullListEmpty_Returns404NotFound()
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
    public async Task GetAllForCurrentUser_CurrentUserIdNull_Returns401Unauthorized()
    {
        //Mock
        _mockCurrentUserService.Setup(c => c.UserId).Returns(null as string);

        //Act
        var serviceResult = await _noteService.GetAllForCurrentUser();

        //Assert
        CommonAssertions.AssertServiceResultFailure(serviceResult, ServiceResultErrorType.Unauthorized);
    }
}