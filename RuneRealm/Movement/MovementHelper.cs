namespace RuneRealm.Movement;

public static class MovementHelper
{
    // These arrays define the change in the X and Y coordinates, respectively, for the eight possible directions.
    public static int[] DIRECTION_DELTA_X = { -1, 0, 1, -1, 1, -1, 0, 1 };
    public static int[] DIRECTION_DELTA_Y = { 1, 1, 1, 0, 0, -1, -1, -1 };


    public static sbyte[] DirectionToClient = { 1, 2, 4, 7, 6, 5, 3, 0 };
    public static sbyte[] DeltaX = { 0, 1, 1, 1, 0, -1, -1, -1 };
    public static sbyte[] DeltaY = { 1, 1, 0, -1, -1, -1, 0, 1 };

    // This method calculates the direction based on the provided deltas for the X and Y coordinates.
    // Possible return values range from 0 to 7, which corresponds to eight possible directions (points on a compass).
    // The method returns -1 if there's no movement (dx and dy are both zero).
    public static int GetDirection(int dx, int dy)
    {
        if (dx < 0)
            return dy < 0 ? 5 : dy > 0 ? 0 : 3;
        if (dx > 0)
            return dy < 0 ? 7 : dy > 0 ? 2 : 4;
        if (dy < 0)
            return 6;
        if (dy > 0)
            return 1;
        return -1;
    }

    public static int Direction(int dx, int dy)
    {
        if (dx < 0)
        {
            if (dy < 0)
                return 5;
            if (dy > 0)
                return 0;
            return 3;
        }

        if (dx > 0)
        {
            if (dy < 0)
                return 7;
            if (dy > 0)
                return 2;
            return 4;
        }

        if (dy < 0)
            return 6;
        if (dy > 0)
            return 1;
        return -1;
    }

    // This method calculates the direction of movement based on a source point (srcX, srcY) and a destination point (x, y).
    // Using Math.Atan2 for more accurate results.
    public static int GetDirection(int srcX, int srcY, int destX, int destY)
    {
        var dx = destX - srcX;
        var dy = destY - srcY;
        var angle = Math.Atan2(dy, dx) * (180.0 / Math.PI);
        var quadrant = dx < 0 ? 2 : 0;
        var octant = (int)((90.0 - angle) / 45.0);
        return dx == 0 ? dy > 0 ? 0 : 8 : (quadrant + octant) & 0xF;
    }

    // public static int DirectionFromDelta(int deltaX, int deltaY)
    // {
    //     for (int i = 0; i < DeltaX.Length; i++)
    //     {
    //         if (DeltaX[i] == deltaX && DeltaY[i] == deltaY)
    //             return DirectionToClient[i];
    //     }
    //
    //     throw new ArgumentException($"Cannot find direction {deltaX} {deltaY}");
    // }

    // This method returns the delta values (dx, dy) for a given direction.
    public static (int dx, int dy) GetDirectionVector(Direction direction)
    {
        return (DIRECTION_DELTA_X[(int)direction], DIRECTION_DELTA_Y[(int)direction]);
    }
    
    public static double EuclideanDistance(double x1, double y1, double x2, double y2)
    {
        return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
    }
    
    public static (int newX, int newY) GetNewLocation(int currentX, int currentY, Direction direction)
    {
        var directionVector = GetDirectionVector(direction);

        var newX = currentX + directionVector.dx;
        var newY = currentY + directionVector.dy;

        return (newX, newY);
    }
}

public enum Direction
{
    NorthWest = 0, // corresponds to (x: -1, y: 1)
    North = 1, // corresponds to (x: 0, y: 1)
    NorthEast = 2, // corresponds to (x: 1, y: 1)
    West = 3, // corresponds to (x: -1, y: 0)
    East = 4, // corresponds to (x: 1, y: 0)
    SouthWest = 5, // corresponds to (x: -1, y: -1)
    South = 6, // corresponds to (x: 0, y: -1)
    SouthEast = 7 // corresponds to (x: 1, y: -1)
}