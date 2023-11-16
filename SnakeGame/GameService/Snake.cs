class Snake
{
    private readonly byte _initSize;
    private List<Point> _parts;
    private Point _movePos;

    public int Growth { get => _parts.Count - _initSize; }

    public Snake()
    {
        _initSize = 3;

        _parts = new List<Point>();
        for (int i = _initSize - 1; i >= 0; i--)
            _parts.Add(new Point(i, 0));

        _movePos = _parts.Last() + MovePositions.Get(Direction.Right);
    }

    public void UpdateMovePosition(Direction direction)
    {
        var newPos = MovePositions.Get(direction);
        if (_movePos.Dot(newPos) >= 0)
        {
            _movePos = newPos;
        }
    }

    public void Move()
    {
        int tailIdx = _parts.Count - 1;

        for (int i = tailIdx; i > 0; i--)
        {
            _parts[i].Assign(_parts[i - 1]);
        }
        _parts[0].Plus(_movePos);
    }

    public void Grow()
    {
        // TODO: consider using linked list?
        _parts.Insert(0, _parts[0] + _movePos);
    }

    public int Length()
    {
        return _parts.Count;
    }

    public List<Point> Parts()
    {
        return _parts;
    }

    public Point NextHeadPosition()
    {
        return _parts[0] + _movePos;
    }

    public bool IsColliding(Point p)
    {
        return _parts.Contains(p);
    }
}


