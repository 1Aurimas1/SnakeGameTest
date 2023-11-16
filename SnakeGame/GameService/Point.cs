public class Point
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point(Point p)
    {
        X = p.X;
        Y = p.Y;
    }

    public void Plus(Point p)
    {
        X += p.X;
        Y += p.Y;
    }

    public void Assign(Point p)
    {
        X = p.X;
        Y = p.Y;
    }

    public int Dot(Point p)
    {
        return X * p.X + Y * p.Y;
    }

    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return !(p1 == p2);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || !(obj is Point)) return false;

        Point otherPoint = (Point)obj;
        return X == otherPoint.X && Y == otherPoint.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}


