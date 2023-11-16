using Microsoft.AspNetCore.SignalR;

public class GameManager
{
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    private static List<GameRoom> _rooms = new List<GameRoom>();

    private Timer _timer;

    static GameManager() { }

    public GameManager(IHubContext<GameHub, IGameClient> hubContext)
    {
        _hubContext = hubContext;
        _timer = new Timer(RunGameRoomLoop, null, 0, 1000);
    }

    private void RunGameRoomLoop(object? state)
    {
        for (int i = _rooms.Count - 1; i >= 0; i--)
        {
            var room = _rooms[i];

            if (room.Status == GameRoomStatus.ReadyToStart)
            {
                _hubContext.Clients.Group(room.Id).ReceiveStateObjects(room.GetGameStates(), true);
                BeginRoomInitialCountdown(room);
            }
            else if (room.Status == GameRoomStatus.Running)
            {
                room.UpdateGameStates();
                _hubContext.Clients.Group(room.Id).ReceiveStateObjects(room.GetGameStates(), false);
            }
            else if (room.Status == GameRoomStatus.Finished)
            {
                // TODO: update users profile stats
                _rooms.RemoveAt(i);
            }
        }
    }

    private async void BeginRoomInitialCountdown(GameRoom room)
    {
        room.PreStartSetup();
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        for (int i = 1; i <= 3; i++)
        {
            await _hubContext.Clients.Group(room.Id).ReceiveCountdown(i);
            await timer.WaitForNextTickAsync();
        }

        room.Start();
    }

    public string JoinGameRoom(string connectionId, string playerName, GameMode gameMode)
    {
        var room = FindOrCreateGameRoom(playerName, gameMode);

        room.Join(playerName);
        _hubContext.Groups.AddToGroupAsync(connectionId, room.Id);
        return room.Id;
    }

    private GameRoom FindOrCreateGameRoom(string playerName, GameMode gameMode)
    {
        GameRoom? room = null;

        if (gameMode == GameMode.Solo)
        {
            room = new GameRoom(1);
            _rooms.Add(room);
        }
        else if (gameMode == GameMode.Duel)
        {
            room = _rooms.Find(g => g.CanJoin(playerName));
            if (room == null)
            {
                room = new GameRoom(2);
                _rooms.Add(room);
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        return room;
    }

    public void UpdatePlayerMovePosition(string gameRoomId, string playerName, Direction direction)
    {
        _rooms.Find(r => r.Id == gameRoomId)?.UpdatePlayerMovePosition(playerName, direction);
    }
}


