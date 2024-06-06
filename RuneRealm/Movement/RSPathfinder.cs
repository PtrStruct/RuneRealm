using RuneRealm.Entities;
using RuneRealm.Environment;

namespace RuneRealm.Movement;

public class RSPathfinder
{
    private const int DEFALT_PATH_LENGTH = 4000;
    private static readonly Random _random = new();

    public static void FindPath(Entity character, int destX, int destY, bool moveNear, int xLength, int yLength)
    {
        if (destX == character.Location.PositionRelativeToOffsetChunkX &&
            destY == character.Location.PositionRelativeToOffsetChunkY && !moveNear)
        {
            if (character is Player player) player.PacketBuilder.SendMessage("ERROR!");

            return;
        }

        var height = character.Location.Z;
        destX = destX - 8 * character.Location.OffsetChunkX;
        destY = destY - 8 * character.Location.OffsetChunkY;

        var via = new int[104][];
        var cost = new int[104][];

        var tileQueueX = new List<int>();
        var tileQueueY = new List<int>();

        for (var i = 0; i < 104; i++)
        {
            via[i] = new int[104];
            cost[i] = new int[104];
        }

        for (var xx = 0; xx < 104; xx++)
        for (var yy = 0; yy < 104; yy++)
            cost[xx][yy] = 99999999;

        var curX = character.Location.PositionRelativeToOffsetChunkX;
        var curY = character.Location.PositionRelativeToOffsetChunkY;

        via[curX][curY] = 99;
        cost[curX][curY] = 0;

        var tail = 0;
        tileQueueX.Add(curX);
        tileQueueY.Add(curY);
        var foundPath = false;

        while (tail != tileQueueX.Count && tileQueueX.Count < DEFALT_PATH_LENGTH)
        {
            curX = tileQueueX.ElementAt(tail);
            curY = tileQueueY.ElementAt(tail);
            var curAbsX = character.Location.OffsetChunkX * 8 + curX;
            var curAbsY = character.Location.OffsetChunkY * 8 + curY;

            if (curX == destX && curY == destY)
            {
                foundPath = true;
                break;
            }

            /* Combat Exit Strategy */
            //if (character.MostRecentCombatTarget != null)
            //{
            //    if (xLength != 0 && yLength != 0 && Region.canInteract(destX, destY, curAbsX, curAbsY, curX, curY,xLength, yLength, 0))
            //    {
            //        foundPath = true;
            //        break;
            //    }
            //}

            tail = (tail + 1) % DEFALT_PATH_LENGTH;
            var thisCost = cost[curX][curY] + 1;
            if (curY > 0 && via[curX][curY - 1] == 0 &&
                (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY - 1);
                via[curX][curY - 1] = 1;
                cost[curX][curY - 1] = thisCost;
            }

            if (curX > 0
                && via[curX - 1][curY] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY);
                via[curX - 1][curY] = 2;
                cost[curX - 1][curY] = thisCost;
            }

            if (curY < 104 - 1
                && via[curX][curY + 1] == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY + 1);
                via[curX][curY + 1] = 4;
                cost[curX][curY + 1] = thisCost;
            }

            if (curX < 104 - 1 && via[curX + 1][curY] == 0 &&
                (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY);
                via[curX + 1][curY] = 8;
                cost[curX + 1][curY] = thisCost;
            }

            if (curX > 0
                && curY > 0
                && via[curX - 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY - 1, height) & 0x128010e) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY - 1);
                via[curX - 1][curY - 1] = 3;
                cost[curX - 1][curY - 1] = thisCost;
            }

            if (curX > 0
                && curY < 104 - 1
                && via[curX - 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY + 1, height) & 0x1280138) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, height) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY + 1);
                via[curX - 1][curY + 1] = 6;
                cost[curX - 1][curY + 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY > 0
                && via[curX + 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY - 1, height) & 0x1280183) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, height) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY - 1);
                via[curX + 1][curY - 1] = 9;
                cost[curX + 1][curY - 1] = thisCost;
            }

            if (curX < 104 - 1
                && curY < 104 - 1
                && via[curX + 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY + 1, height) & 0x12801e0) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, height) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, height) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY + 1);
                via[curX + 1][curY + 1] = 12;
                cost[curX + 1][curY + 1] = thisCost;
            }
        }

        if (!foundPath)
        {
            if (moveNear)
            {
                var i_223_ = 1000;
                var thisCost = 100;
                var i_225_ = 10;
                for (var x = destX - i_225_; x <= destX + i_225_; x++)
                for (var y = destY - i_225_; y <= destY + i_225_; y++)
                    if (x >= 0 && y >= 0 && x < 104 && y < 104 && cost[x][y] < 100)
                    {
                        var i_228_ = 0;
                        if (x < destX)
                            i_228_ = destX - x;
                        else if (x > destX + xLength - 1) i_228_ = x - (destX + xLength - 1);

                        var i_229_ = 0;
                        if (y < destY)
                            i_229_ = destY - y;
                        else if (y > destY + yLength - 1) i_229_ = y - (destY + yLength - 1);

                        var i_230_ = i_228_ * i_228_ + i_229_ * i_229_;
                        if (i_230_ < i_223_ || (i_230_ == i_223_ && cost[x][y] < thisCost))
                        {
                            i_223_ = i_230_;
                            thisCost = cost[x][y];
                            curX = x;
                            curY = y;
                        }
                    }

                if (i_223_ == 1000) return;
            }
            else
            {
                return;
            }
        }

        tail = 0;
        tileQueueX[tail] = curX;
        tileQueueY[tail++] = curY;

        int l5;
        for (var j5 = l5 = via[curX][curY];
             curX != character.Location.PositionRelativeToOffsetChunkX ||
             curY != character.Location.PositionRelativeToOffsetChunkY;
             j5 = via[curX][curY])
        {
            if (j5 != l5)
            {
                l5 = j5;
                tileQueueX[tail] = curX;
                tileQueueY[tail++] = curY;
            }

            if ((j5 & 2) != 0)
                curX++;
            else if ((j5 & 8) != 0) curX--;

            if ((j5 & 1) != 0)
                curY++;
            else if ((j5 & 4) != 0) curY--;
        }

        var size = tail--;
        var pathX = character.Location.OffsetChunkX * 8 + tileQueueX[tail];
        var pathY = character.Location.OffsetChunkY * 8 + tileQueueY[tail];

        character.MovementHandler.AddToPath(new Location(pathX, pathY, character.Location.Z));

        for (var i = 1; i < size; i++)
        {
            tail--;
            pathX = character.Location.OffsetChunkX * 8 + tileQueueX[tail];
            pathY = character.Location.OffsetChunkY * 8 + tileQueueY[tail];
            character.MovementHandler.AddToPath(new Location(pathX, pathY, character.Location.Z));
        }
    }

    public static bool IsProjectilePathClear(int x0, int y0, int z, int x1, int y1)
    {
        var deltaX = x1 - x0;
        var deltaY = y1 - y0;

        double error = 0;
        var deltaError = Math.Abs(
            deltaY / (deltaX == 0 ? deltaY : (double)deltaX));

        var x = x0;
        var y = y0;

        var pX = x;
        var pY = y;

        var incrX = x0 < x1;
        var incrY = y0 < y1;

        if (!IsAccessible(x0, y0, z, pX, pY))
        {
            return false;
        }

        while (true)
        {
            if (x != x1) x += incrX ? 1 : -1;

            if (y != y1)
            {
                error += deltaError;

                if (error >= 0.5)
                {
                    y += incrY ? 1 : -1;
                    error -= 1;
                }
            }

            if (!Shootable(x, y, z, pX, pY)) return false;

            if (incrX && incrY
                      && x >= x1 && y >= y1)
                break;
            if (!incrX && !incrY
                       && x <= x1 && y <= y1)
                break;
            if (!incrX && incrY
                       && x <= x1 && y >= y1)
                break;
            if (incrX && !incrY
                      && x >= x1 && y <= y1)
                break;

            pX = x;
            pY = y;
        }

        return true;
    }

    private static bool Shootable(int x, int y, int z, int px, int py)
    {
        if (x == px && y == py) return true;

        var delta1 = Location.Delta(new Location(x, y, z), new Location(px, py, z));
        var delta2 = Location.Delta(new Location(px, py, z), new Location(x, y, z));

        var dir = MovementHelper.GetDirection(delta1.X, delta1.Y);
        var dir2 = MovementHelper.GetDirection(delta2.X, delta2.Y);

        if (dir == -1 || dir2 == -1) return false;

        return (Region.CanMove(x, y, z, (Direction)dir) && Region.CanMove(px, py, z, (Direction)dir2))
               || (Region.CanShoot(x, y, z, dir) && Region.CanShoot(px, py, z, dir2));
    }
    
    public static bool IsAccessible(int x, int y, int z, int destX, int destY)
    {
        Location location = new Location(x, y, z);

        if (destX == location.PositionRelativeToOffsetChunkX &&
            destY == location.PositionRelativeToOffsetChunkY)
        {
            return false;
        }

        int[][] via = new int[104][];
        int[][] cost = new int[104][];

        for (var i = 0; i < 104; i++)
        {
            via[i] = new int[104];
            cost[i] = new int[104];
        }

        List<int> tileQueueX = new List<int>(10000);
        List<int> tileQueueY = new List<int>(10000);

        int curX = location.PositionRelativeToOffsetChunkX;
        int curY = location.PositionRelativeToOffsetChunkY;

        via[curX][curY] = 99;
        cost[curX][curY] = 1;

        var tail = 0;

        tileQueueX.Add(curX);
        tileQueueY.Add(curY);

        destX = destX - 8 * location.OffsetChunkX;
        destY = destY - 8 * location.OffsetChunkY;

        while (tail != tileQueueX.Count() && tileQueueX.Count() < 104)
        {
            curX = tileQueueX.ElementAt(tail);
            curY = tileQueueY.ElementAt(tail);

            int curAbsX = location.OffsetChunkX * 8 + curX;
            int curAbsY = location.OffsetChunkY * 8 + curY;

            if (curX == destX && curY == destY)
            {
                return true;
            }

            tail = (tail + 1) % 104;

            int thisCost = cost[curX][curY] + 1;

            if (curY > 0 && via[curX][curY - 1] == 0
                         && (Region.GetClipping(curAbsX, curAbsY - 1, z) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY - 1);
                via[curX][curY - 1] = 1;
                cost[curX][curY - 1] = thisCost;
            }

            if (curX > 0 && via[curX - 1][curY] == 0
                         && (Region.GetClipping(curAbsX - 1, curAbsY, z) & 0x1280108) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY);
                via[curX - 1][curY] = 2;
                cost[curX - 1][curY] = thisCost;
            }

            if (curY < 104 - 1 && via[curX][curY + 1] == 0
                               && (Region.GetClipping(curAbsX, curAbsY + 1, z) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX);
                tileQueueY.Add(curY + 1);
                via[curX][curY + 1] = 4;
                cost[curX][curY + 1] = thisCost;
            }

            if (curX < 104 - 1 && via[curX + 1][curY] == 0
                               && (Region.GetClipping(curAbsX + 1, curAbsY, z) & 0x1280180) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY);
                via[curX + 1][curY] = 8;
                cost[curX + 1][curY] = thisCost;
            }

            // Diagonal movements

            if (curX > 0 && curY > 0 && via[curX - 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY - 1, z) & 0x128010e) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, z) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, z) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY - 1);
                via[curX - 1][curY - 1] = 3;
                cost[curX - 1][curY - 1] = thisCost;
            }

            if (curX > 0 && curY < 104 - 1 && via[curX - 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY + 1, z) & 0x1280138) == 0
                && (Region.GetClipping(curAbsX - 1, curAbsY, z) & 0x1280108) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, z) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX - 1);
                tileQueueY.Add(curY + 1);
                via[curX - 1][curY + 1] = 6;
                cost[curX - 1][curY + 1] = thisCost;
            }

            if (curX < 104 - 1 && curY > 0 && via[curX + 1][curY - 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY - 1, z) & 0x1280183) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, z) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY - 1, z) & 0x1280102) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY - 1);
                via[curX + 1][curY - 1] = 9;
                cost[curX + 1][curY - 1] = thisCost;
            }

            if (curX < 104 - 1 && curY < 104 - 1 && via[curX + 1][curY + 1] == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY + 1, z) & 0x12801e0) == 0
                && (Region.GetClipping(curAbsX + 1, curAbsY, z) & 0x1280180) == 0
                && (Region.GetClipping(curAbsX, curAbsY + 1, z) & 0x1280120) == 0)
            {
                tileQueueX.Add(curX + 1);
                tileQueueY.Add(curY + 1);
                via[curX + 1][curY + 1] = 12;
                cost[curX + 1][curY + 1] = thisCost;
            }
        }

        return false;
    }
    
}