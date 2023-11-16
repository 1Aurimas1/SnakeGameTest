static class Grid
{
    public static readonly byte Rows, Cols;
    public static readonly int Size;

    static Grid()
    {
        Rows = Cols = 12;
        Size = Rows * Cols;
    }

    public static bool IsWithinBoundaries(Point point)
    {
        return (point.X < 0 || point.X >= Cols ||
                point.Y < 0 || point.Y >= Rows) ?
                false : true;
    }
}


