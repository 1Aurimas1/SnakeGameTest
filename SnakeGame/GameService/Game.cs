enum GameStatus
{
    Initialized,
    Running,
    Finished,
}

class Game
{
    private Point _food;

    private bool _toUpdateFood;

    public string Player { get; }
    public Snake Snake { get; private set; }
    public GameStatus Status { get; private set; }

    public Game(string playerName)
    {
        Snake = new Snake();
        _food = SpawnFood();

        _toUpdateFood = true;
        Status = GameStatus.Initialized;

        Player = playerName;
    }

    public bool Start()
    {
        if (Status != GameStatus.Initialized) return false;

        Status = GameStatus.Running;
        return true;
    }

    public void UpdateState()
    {
        if (Status != GameStatus.Running) return;
        ResetUpdateChecks();

        Point nextPos = Snake.NextHeadPosition();
        if (IsColliding(nextPos))
        {
            Status = GameStatus.Finished;
        }
        else if (_food == nextPos)
        {
            Snake.Grow();
            _food = SpawnFood();
            _toUpdateFood = true;
        }
        else if (Grid.Size == Snake.Length() + 1)
        {
            Status = GameStatus.Finished;
        }
        else
        {
            Snake.Move();
        }
    }

    private void ResetUpdateChecks()
    {
        _toUpdateFood = false;
    }

    public GameDto GetCurrentState()
    {
        return new GameDto()
        {
            PlayerName = Player,
            SnakeParts = Snake.Parts(),
            Score = _toUpdateFood ? Snake.Growth : null,
            Food = _toUpdateFood ? _food : null,
        };
    }

    private bool IsColliding(Point p)
    {
        return !Grid.IsWithinBoundaries(p) || Snake.IsColliding(p);
    }

    private Point SpawnFood()
    {
        Point[] emptyCells = FindEmptyCells();

        Random rnd = new Random();
        int idx = rnd.Next(0, emptyCells.Length);

        return emptyCells[idx];
    }

    private Point[] FindEmptyCells()
    {
        Point[] emptyCells = new Point[Grid.Size - Snake.Length()];

        int idx = 0;
        for (int i = 0; i < Grid.Rows; i++)
        {
            for (int j = 0; j < Grid.Cols; j++)
            {
                Point cell = new Point(i, j);
                if (!Snake.IsColliding(cell))
                {
                    emptyCells[idx++] = cell;
                }
            }
        }

        return emptyCells;
    }
}


