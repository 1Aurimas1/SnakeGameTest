using Microsoft.AspNetCore.SignalR;
using Moq;

namespace SnakeGame.Tests;

public class GameHubTests
{
    [Theory]
    [InlineData("TestName", GameMode.Solo)]
    [InlineData("123456789", GameMode.Duel)]
    public async Task JoinGame_WithValidArguments_ReturnsValidGuidId(
        string userName,
        GameMode gameMode
    )
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

        GameManager gameManager = new(mockHubContext.Object);

        var hub = new GameHub(gameManager)
        {
            Clients = mockClients.Object,
            Context = mockContext.Object
        };

        var roomId = await hub.JoinGame(userName, gameMode);

        mockHubContext.Verify(
            manager =>
                manager.Groups.AddToGroupAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        Assert.IsType<string>(roomId);
        Assert.NotNull(roomId);

        Guid guidOutput;
        bool isValidGuid = Guid.TryParse(roomId, out guidOutput);
        Assert.True(isValidGuid, "Returned string is not a valid GUID.");
    }

    [Fact]
    public async Task InitGrid_ReturnsValidTuple()
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

        GameManager gameManager = new(mockHubContext.Object);

        var hub = new GameHub(gameManager)
        {
            Clients = mockClients.Object,
            Context = mockContext.Object
        };

        var dimensions = await hub.InitGrid();

        Assert.IsType<Tuple<int, int>>(dimensions);
        Assert.NotNull(dimensions);

        Assert.Equal(12, dimensions.Item1);
        Assert.Equal(12, dimensions.Item2);
    }

    [Fact]
    public async Task SendInput_WithValidArguments_ShouldCallUpdatePlayerMovePositionOnGameManager()
    {
        Mock<IHubCallerClients<IGameClient>> mockClients = new();
        Mock<HubCallerContext> mockContext = new();
        Mock<IHubContext<GameHub, IGameClient>> mockHubContext = new();
        Mock<IGameManager> mockGameManager = new();

        mockContext.Setup(context => context.ConnectionId).Returns("1");
        mockGameManager.Setup(
            m =>
                m.UpdatePlayerMovePosition(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Direction>()
                )
        );

        var hub = new GameHub(mockGameManager.Object)
        {
            Clients = mockClients.Object,
            Context = mockContext.Object
        };

        await hub.SendInput("1", "TestName", Direction.Up);

        mockGameManager.Verify(
            m =>
                m.UpdatePlayerMovePosition(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Direction>()
                ),
            Times.Once
        );
    }
}
