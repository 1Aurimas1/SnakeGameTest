using Microsoft.AspNetCore.SignalR;
using Moq;
using SnakeGame.GameService;
using SnakeGame.Hubs;


namespace SnakeGame.Tests;

public sealed class GameManagerTests
{
    private readonly GameManager _sut;

    public GameManagerTests()
    {
        Mock<IHubCallerClients<IGameClient>> mockClients = new();
        Mock<HubCallerContext> mockContext = new();
        Mock<IHubContext<GameHub, IGameClient>> mockHubContext = new();

        mockContext.Setup(context => context.ConnectionId).Returns("1");
        mockHubContext.Setup(
            c =>
                c.Clients
                    .Group(It.IsAny<string>())
                    .ReceiveStateObjects(It.IsAny<List<GameDto>>(), It.IsAny<bool>())
        );
        mockHubContext
            .Setup(
                c =>
                    c.Groups.AddToGroupAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .Returns(Task.CompletedTask);
        _sut = new GameManager( mockHubContext.Object );
    }


    [Fact]
    public void JoinGameRoom_ShouldReturnString_WhenGameRoomIsCreated()
    {
        // Arrange
        string connectionId = "132";
        string Name = "Dabusy";
        GameMode mode = GameMode.Solo;
        // Act
        string id = _sut.JoinGameRoom( connectionId, Name, mode );

        // Assert
        Assert.NotNull( id );
    }

    [Fact]
    public void JoinGameRoom_ShouldReturnString_WhenGameRoomIsJoined()
    {
        // Arrange
        string connectionId = "132";
        string Name = "Dabusy";
        GameMode mode = GameMode.Duel;
        // Act
        string id = _sut.JoinGameRoom(connectionId, Name, mode);
        string id2 = _sut.JoinGameRoom(connectionId, "Mark", mode);
        // Assert
        Assert.NotNull(id);
        Assert.NotNull(id2);
        Assert.Equal(id, id2);
    }

    [Fact]
    public void JoinGameRoom_ShouldReturnError_WhenGameModeIsFalse()
    {
        // Arrange
        string connectionId = "132";
        string Name = "Dabusy";
        int fakeMode = 3;
        // Act
        Assert.Throws<NotImplementedException>(() => _sut.JoinGameRoom(connectionId, Name, (GameMode)fakeMode));
    }

}
