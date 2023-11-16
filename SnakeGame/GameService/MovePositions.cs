static class MovePositions
{
    //public static readonly Point Up, Down, Left, Right;
    private static readonly Dictionary<Direction, Point> _positions;

    static MovePositions()
    {
        _positions = new Dictionary<Direction, Point>();
        _positions.Add(Direction.Up, new Point(0, 1));
        _positions.Add(Direction.Down, new Point(0, -1));
        _positions.Add(Direction.Left, new Point(-1, 0));
        _positions.Add(Direction.Right, new Point(1, 0));
        //Up = new Point(0, 1);
        //Down = new Point(0, -1);
        //Left = new Point(-1, 0);
        //Right = new Point(1, 0);
    }

    public static Point Get(Direction direction)
    {
        return _positions[direction];
    }
}


