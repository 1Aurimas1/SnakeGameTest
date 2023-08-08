public interface IGameClient
{
    Task ReceiveSnake(List<Point> parts);
    Task ReceiveFood(Point position);
    Task ReceiveStateObjects(List<GameDto> gameStates);
}


