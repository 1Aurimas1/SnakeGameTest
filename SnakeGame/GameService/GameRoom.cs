enum GameRoomStatus
{
    OpenToJoin,
    ReadyToStart,
    Starting,
    Running,
    Finished,
}

class GameRoom
{
    private readonly uint _maxPlayers;

    public string Id { get; }
    public List<Game> Games { get; private set; }
    public List<GameDto> GameStates { get; private set; }

    private bool IsFull => Games.Count >= _maxPlayers;
    public GameRoomStatus Status { get; private set; }

    public GameRoom(uint maxPlayers = 2)
    {
        _maxPlayers = maxPlayers;
        GameStates = new List<GameDto>();

        Id = Guid.NewGuid().ToString();
        Games = new List<Game>();

        Status = GameRoomStatus.OpenToJoin;
    }


    public bool CanJoin(string playerName)
    {
        return Status == GameRoomStatus.OpenToJoin && !Games.Any(g => g.Player == playerName);
    }

    public bool Join(string playerName)
    {
        if (Status != GameRoomStatus.OpenToJoin) return false;

        var game = new Game(playerName);
        Games.Add(game);
        GameStates.Add(game.GetCurrentState());

        if (IsFull) Status = GameRoomStatus.ReadyToStart;

        return true;
    }

    public bool PreStartSetup()
    {
        if (Status != GameRoomStatus.ReadyToStart) return false;

        Status = GameRoomStatus.Starting;
        Games.ForEach(g => g.Start());

        return true;
    }

    public bool Start()
    {
        if (Status != GameRoomStatus.Starting) return false;

        Status = GameRoomStatus.Running;

        return true;
    }

    public List<GameDto> GetGameStates()
    {
        return GameStates;
    }

    public void UpdateGameStates()
    {
        int finishedGames = 0;

        for (int i = 0; i < Games.Count; i++)
        {
            if (Games[i].Status == GameStatus.Running)
            {
                Games[i].UpdateState();
                GameStates[i] = Games[i].GetCurrentState();
            }
            else if (Games[i].Status == GameStatus.Finished)
            {
                finishedGames++;
            }
        }

        if (finishedGames >= Games.Count)
            Status = GameRoomStatus.Finished;
    }

    public void UpdatePlayerMovePosition(string playerName, Direction direction)
    {
        Games.Find(g => g.Player == playerName)?.Snake.UpdateMovePosition(direction);
    }
}
