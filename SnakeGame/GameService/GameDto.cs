public class GameDto
{
    public string PlayerName { get; set; }
    public List<Point> SnakeParts { get; set; }
    public Point? Food { get; set; }
    public int? Score { get; set; }
}


