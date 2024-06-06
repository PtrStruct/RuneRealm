namespace RuneRealm.Movement;

public class Waypoint
{
    public Waypoint(int x, int y, int direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    public int X { get; }
    public int Y { get; }
    public int Direction { get; }
}