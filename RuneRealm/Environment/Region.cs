

using RuneRealm.Data;
using RuneRealm.Data.ObjectsDef;
using RuneRealm.Movement;

namespace RuneRealm.Environment;

public class Region
{
    public const int PROJECTILE_NORTH_WEST_BLOCKED = 0x200;
    public const int PROJECTILE_NORTH_BLOCKED = 0x400;
    public const int PROJECTILE_NORTH_EAST_BLOCKED = 0x800;
    public const int PROJECTILE_EAST_BLOCKED = 0x1000;
    public const int PROJECTILE_SOUTH_EAST_BLOCKED = 0x2000;
    public const int PROJECTILE_SOUTH_BLOCKED = 0x4000;
    public const int PROJECTILE_SOUTH_WEST_BLOCKED = 0x8000;
    public const int PROJECTILE_WEST_BLOCKED = 0x10000;
    public const int PROJECTILE_TILE_BLOCKED = 0x20000;
    public const int UNKNOWN = 0x80000;
    public const int BLOCKED_TILE = 0x200000;
    public const int UNLOADED_TILE = 0x1000000;
    public const int OCEAN_TILE = 2097152;
    private static int i = 0;

    public Region(int id, bool members)
    {
        Id = id;
        Members = members;
    }

    private int[][][] _clips { get; } = new int[4][][];
    private int[][][] _projectileClips { get; } = new int[4][][];
    private List<Objects> _realObjects { get; } = new();

    public int Id { get; }

    public bool Members { get; }

    public static Region GetRegion(int x, int y)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region)) return region;

        return null;
    }

    public static int GetRegionId(int x, int y)
    {
        var regionX = x >> 3;
        var regionY = y >> 3;
        var regionId = ((regionX / 8) << 8) + regionY / 8;
        return regionId;
    }

    private void AddClip(int x, int y, int height, int shift)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_clips[height] == null)
        {
            _clips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _clips[height][i] = new int[64];
        }

        _clips[height][x - regionAbsX][y - regionAbsY] |= shift;
    }

    private void AddProjectileClip(int x, int y, int height, int shift)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_projectileClips[height] == null)
        {
            _projectileClips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _projectileClips[height][i] = new int[64];
        }

        _projectileClips[height][x - regionAbsX][y - regionAbsY] |= shift;
    }

    private int GetClip(int x, int y, int height)
    {
        int zx;
        int zy;
        try
        {
            var regionAbsX = (Id >> 8) * 64;
            var regionAbsY = (Id & 0xff) * 64;
            if (_clips[height] == null) return 0;
            zx = x - regionAbsX;
            zy = y - regionAbsY;

            if (zx >= 0 && zx <= 64 && zy >= 0 && zy <= 64) return _clips[height][x - regionAbsX][y - regionAbsY];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return -1;
    }

    private int GetProjectileClip(int x, int y, int height)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_projectileClips[height] == null) return 0;

        return _projectileClips[height][x - regionAbsX][y - regionAbsY];
    }

    public static void AddClipping(int x, int y, int height, int shift)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region))
            region.AddClip(x, y, height, shift);
    }

    private static void AddProjectileClipping(int x, int y, int height, int shift)
    {
        if (RegionFactory.GetRegions().TryGetValue(GetRegionId(x, y), out var region))
            region.AddProjectileClip(x, y, height, shift);
    }

    public static int GetClipping(int x, int y, int height)
    {
        if (height > 3) height = 0;

        foreach (var r in RegionFactory.GetRegions())
        {
            var rId = r.Value.Id;
            var regionId = GetRegionId(x, y);
            if (rId == regionId)
                return r.Value.GetClip(x, y, height);
        }

        return 0;
    }

    public static int GetProjectileClipping(int x, int y, int height)
    {
        if (height > 3) height = 0;

        foreach (var r in RegionFactory.GetRegions())
            if (r.Value.Id == GetRegionId(x, y))
                return r.Value.GetProjectileClip(x, y, height);

        return 0;
    }

    private static void AddClippingForSolidObject(int x, int y, int height, int xLength, int yLength, bool flag)
    {
        var clipping = 256;
        if (flag) clipping += 0x20000;

        for (var i = x; i < x + xLength; i++)
        for (var i2 = y; i2 < y + yLength; i2++)
            AddClipping(i, i2, height, clipping);
    }

    private static void AddProjectileClippingForSolidObject(int x, int y, int height, int xLength, int yLength,
        bool flag)
    {
        var clipping = 256;
        if (flag) clipping += 0x20000;

        for (var i = x; i < x + xLength; i++)
        for (var j = y; j < y + yLength; j++)
            AddProjectileClipping(i, j, height, clipping);
    }

    private static void AddClippingForVariableObject(int x, int y, int height, int type, int direction, bool flag)
    {
        if (type == 0)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 128);
                AddClipping(x - 1, y, height, 8);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 2);
                AddClipping(x, y + 1, height, 32);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 8);
                AddClipping(x + 1, y, height, 128);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 32);
                AddClipping(x, y - 1, height, 2);
            }
        }
        else if (type == 1 || type == 3)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 1);
                AddClipping(x - 1, y, height, 16);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 4);
                AddClipping(x + 1, y + 1, height, 64);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 16);
                AddClipping(x + 1, y - 1, height, 1);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 64);
                AddClipping(x - 1, y - 1, height, 4);
            }
        }
        else if (type == 2)
        {
            if (direction == 0)
            {
                AddClipping(x, y, height, 130);
                AddClipping(x - 1, y, height, 8);
                AddClipping(x, y + 1, height, 32);
            }
            else if (direction == 1)
            {
                AddClipping(x, y, height, 10);
                AddClipping(x, y + 1, height, 32);
                AddClipping(x + 1, y, height, 128);
            }
            else if (direction == 2)
            {
                AddClipping(x, y, height, 40);
                AddClipping(x + 1, y, height, 128);
                AddClipping(x, y - 1, height, 2);
            }
            else if (direction == 3)
            {
                AddClipping(x, y, height, 160);
                AddClipping(x, y - 1, height, 2);
                AddClipping(x - 1, y, height, 8);
            }
        }

        if (flag)
        {
            if (type == 0)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 65536);
                    AddClipping(x - 1, y, height, 4096);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 1024);
                    AddClipping(x, y + 1, height, 16384);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 4096);
                    AddClipping(x + 1, y, height, 65536);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 16384);
                    AddClipping(x, y - 1, height, 1024);
                }
            }

            if (type == 1 || type == 3)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 512);
                    AddClipping(x - 1, y + 1, height, 8192);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 2048);
                    AddClipping(x + 1, y + 1, height, 32768);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 8192);
                    AddClipping(x + 1, y + 1, height, 512);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 32768);
                    AddClipping(x - 1, y - 1, height, 2048);
                }
            }
            else if (type == 2)
            {
                if (direction == 0)
                {
                    AddClipping(x, y, height, 66560);
                    AddClipping(x - 1, y, height, 4096);
                    AddClipping(x, y + 1, height, 16384);
                }
                else if (direction == 1)
                {
                    AddClipping(x, y, height, 5120);
                    AddClipping(x, y + 1, height, 16384);
                    AddClipping(x + 1, y, height, 65536);
                }
                else if (direction == 2)
                {
                    AddClipping(x, y, height, 20480);
                    AddClipping(x + 1, y, height, 65536);
                    AddClipping(x, y - 1, height, 1024);
                }
                else if (direction == 3)
                {
                    AddClipping(x, y, height, 81920);
                    AddClipping(x, y - 1, height, 1024);
                    AddClipping(x - 1, y, height, 4096);
                }
            }
        }
    }

    private static void AddProjectileClippingForVariableObject(int x, int y, int height, int type, int direction,
        bool flag)
    {
        if (type == 0)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 128);
                AddProjectileClipping(x - 1, y, height, 8);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 2);
                AddProjectileClipping(x, y + 1, height, 32);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 8);
                AddProjectileClipping(x + 1, y, height, 128);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 32);
                AddProjectileClipping(x, y - 1, height, 2);
            }
        }
        else if (type == 1 || type == 3)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 1);
                AddProjectileClipping(x - 1, y, height, 16);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 4);
                AddProjectileClipping(x + 1, y + 1, height, 64);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 16);
                AddProjectileClipping(x + 1, y - 1, height, 1);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 64);
                AddProjectileClipping(x - 1, y - 1, height, 4);
            }
        }
        else if (type == 2)
        {
            if (direction == 0)
            {
                AddProjectileClipping(x, y, height, 130);
                AddProjectileClipping(x - 1, y, height, 8);
                AddProjectileClipping(x, y + 1, height, 32);
            }
            else if (direction == 1)
            {
                AddProjectileClipping(x, y, height, 10);
                AddProjectileClipping(x, y + 1, height, 32);
                AddProjectileClipping(x + 1, y, height, 128);
            }
            else if (direction == 2)
            {
                AddProjectileClipping(x, y, height, 40);
                AddProjectileClipping(x + 1, y, height, 128);
                AddProjectileClipping(x, y - 1, height, 2);
            }
            else if (direction == 3)
            {
                AddProjectileClipping(x, y, height, 160);
                AddProjectileClipping(x, y - 1, height, 2);
                AddProjectileClipping(x - 1, y, height, 8);
            }
        }


        if (flag)
        {
            if (type == 0)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 65536);
                    AddProjectileClipping(x - 1, y, height, 4096);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 1024);
                    AddProjectileClipping(x, y + 1, height, 16384);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 4096);
                    AddProjectileClipping(x + 1, y, height, 65536);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 16384);
                    AddProjectileClipping(x, y - 1, height, 1024);
                }
            }

            if (type == 1 || type == 3)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 512);
                    AddProjectileClipping(x - 1, y + 1, height, 8192);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 2048);
                    AddProjectileClipping(x + 1, y + 1, height, 32768);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 8192);
                    AddProjectileClipping(x + 1, y + 1, height, 512);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 32768);
                    AddProjectileClipping(x - 1, y - 1, height, 2048);
                }
            }
            else if (type == 2)
            {
                if (direction == 0)
                {
                    AddProjectileClipping(x, y, height, 66560);
                    AddProjectileClipping(x - 1, y, height, 4096);
                    AddProjectileClipping(x, y + 1, height, 16384);
                }
                else if (direction == 1)
                {
                    AddProjectileClipping(x, y, height, 5120);
                    AddProjectileClipping(x, y + 1, height, 16384);
                    AddProjectileClipping(x + 1, y, height, 65536);
                }
                else if (direction == 2)
                {
                    AddProjectileClipping(x, y, height, 20480);
                    AddProjectileClipping(x + 1, y, height, 65536);
                    AddProjectileClipping(x, y - 1, height, 1024);
                }
                else if (direction == 3)
                {
                    AddProjectileClipping(x, y, height, 81920);
                    AddProjectileClipping(x, y - 1, height, 1024);
                    AddProjectileClipping(x - 1, y, height, 4096);
                }
            }
        }
    }

    public static void AddObject(int objectId, int x, int y, int height, int type, int direction, bool startUp)
    {
        var item = ObjectDefinition.Lookup(objectId);
        if (ObjectDefinition.Lookup(objectId) == null)
        {
        }

        int xLength;
        int yLength;

        if (direction != 1 && direction != 3)
        {
            xLength = ObjectDefinition.Lookup(objectId).Width;
            yLength = ObjectDefinition.Lookup(objectId).Length;
        }
        else
        {
            xLength = ObjectDefinition.Lookup(objectId).Length;
            yLength = ObjectDefinition.Lookup(objectId).Width;
        }

        if (type == 22)
        {
            if (ObjectDefinition.Lookup(objectId).IsInteractive && ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClipping(x, y, height, 0x200000);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable) AddProjectileClipping(x, y, height, 0x200000);
            }
        }
        else if (type >= 9)
        {
            if (ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClippingForSolidObject(x, y, height, xLength, yLength, ObjectDefinition.Lookup(objectId).IsClipped);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable)
                    AddProjectileClippingForSolidObject(x, y, height, xLength, yLength,
                        ObjectDefinition.Lookup(objectId).IsClipped);
            }
        }
        else if (type >= 0 && type <= 3)
        {
            if (ObjectDefinition.Lookup(objectId).IsSolid)
            {
                AddClippingForVariableObject(x, y, height, type, direction,
                    ObjectDefinition.Lookup(objectId).IsClipped);

                if (ObjectDefinition.Lookup(objectId).IsImpenetrable)
                    AddProjectileClippingForVariableObject(x, y, height, type, direction,
                        ObjectDefinition.Lookup(objectId).IsClipped);
            }
        }

        var r = GetRegion(x, y);

        if (r != null)
        {
            if (startUp)
                r._realObjects.Add(new Objects(objectId, x, y, height, direction, type, 0));
            else if (!ObjectExists(objectId, x, y, height))
                r._realObjects.Add(new Objects(objectId, x, y, height, direction, type, 0));
        }
    }


    public static bool Blocked(int x, int y, int z)
    {
        return (GetClipping(x, y, z) & 0x1000000) != 0;
    }

    public static bool BlockedNorth(int x, int y, int z)
    {
        return (GetClipping(x, y + 1, z) & 0x1280120) != 0;
    }

    public static bool BlockedEast(int x, int y, int z)
    {
        return (GetClipping(x + 1, y, z) & 0x1280180) != 0;
    }

    public static bool BlockedWest(int x, int y, int z)
    {
        return (GetClipping(x - 1, y, z) & 0x1280108) != 0;
    }

    public static bool BlockedSouth(int x, int y, int z)
    {
        return (GetClipping(x, y - 1, z) & 0x1280102) != 0;
    }

    public static bool BlockedNorthEast(int x, int y, int z)
    {
        return (GetClipping(x + 1, y + 1, z) & 0x12801e0) != 0;
    }

    public static bool BlockedNorthWest(int x, int y, int z)
    {
        return (GetClipping(x - 1, y + 1, z) & 0x1280138) != 0;
    }

    public static bool BlockedSouthEast(int x, int y, int z)
    {
        return (GetClipping(x + 1, y - 1, z) & 0x1280183) != 0;
    }

    public static bool BlockedSouthWest(int x, int y, int z)
    {
        return (GetClipping(x - 1, y - 1, z) & 0x128010e) != 0;
    }

    public static bool ProjectileBlockedNorth(int x, int y, int z)
    {
        return (GetProjectileClipping(x, y + 1, z) & 0x1280120) != 0;
    }

    public static bool ProjectileBlockedEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y, z) & 0x1280180) != 0;
    }

    public static bool ProjectileBlockedSouth(int x, int y, int z)
    {
        return (GetProjectileClipping(x, y - 1, z) & 0x1280102) != 0;
    }

    public static bool ProjectileBlockedWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y, z) & 0x1280108) != 0;
    }

    public static bool ProjectileBlockedNorthEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y + 1, z) & 0x12801e0) != 0;
    }

    public static bool ProjectileBlockedNorthWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y + 1, z) & 0x1280138) != 0;
    }

    public static bool ProjectileBlockedSouthEast(int x, int y, int z)
    {
        return (GetProjectileClipping(x + 1, y - 1, z) & 0x1280183) != 0;
    }

    public static bool ProjectileBlockedSouthWest(int x, int y, int z)
    {
        return (GetProjectileClipping(x - 1, y - 1, z) & 0x128010e) != 0;
    }

    public static bool GetClipping(int x, int y, int height, int moveTypeX, int moveTypeY)
    {
        try
        {
            if (height > 3) height = 0;

            var checkX = x + moveTypeX;
            var checkY = y + moveTypeY;
            if (moveTypeX == -1 && moveTypeY == 0) return (GetClipping(x, y, height) & 0x1280108) == 0;

            if (moveTypeX == 1 && moveTypeY == 0) return (GetClipping(x, y, height) & 0x1280180) == 0;

            if (moveTypeX == 0 && moveTypeY == -1) return (GetClipping(x, y, height) & 0x1280102) == 0;

            if (moveTypeX == 0 && moveTypeY == 1) return (GetClipping(x, y, height) & 0x1280120) == 0;

            if (moveTypeX == -1 && moveTypeY == -1)
                return (GetClipping(x, y, height) & 0x128010e) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280102) == 0;

            if (moveTypeX == 1 && moveTypeY == -1)
                return (GetClipping(x, y, height) & 0x1280183) == 0
                       && (GetClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetClipping(checkX, checkY - 1, height) & 0x1280102) == 0;

            if (moveTypeX == -1 && moveTypeY == 1)
                return (GetClipping(x, y, height) & 0x1280138) == 0
                       && (GetClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            if (moveTypeX == 1 && moveTypeY == 1)
                return (GetClipping(x, y, height) & 0x12801e0) == 0
                       && (GetClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            Console.WriteLine("[FATAL ERROR]: At getClipping: " + x + ", "
                              + y + ", " + height + ", " + moveTypeX + ", "
                              + moveTypeY);
            return false;
        }
        catch (Exception e)
        {
            return true;
        }
    }


    public static bool GetClippingP(int x, int y, int height, int moveTypeX, int moveTypeY)
    {
        try
        {
            if (height > 3) height = 0;

            var checkX = x + moveTypeX;
            var checkY = y + moveTypeY;
            if (moveTypeX == -1 && moveTypeY == 0) return (GetProjectileClipping(x, y, height) & 0x1280108) == 0;

            if (moveTypeX == 1 && moveTypeY == 0) return (GetProjectileClipping(x, y, height) & 0x1280180) == 0;

            if (moveTypeX == 0 && moveTypeY == -1) return (GetProjectileClipping(x, y, height) & 0x1280102) == 0;

            if (moveTypeX == 0 && moveTypeY == 1) return (GetProjectileClipping(x, y, height) & 0x1280120) == 0;

            if (moveTypeX == -1 && moveTypeY == -1)
                return (GetProjectileClipping(x, y, height) & 0x128010e) == 0
                       && (GetProjectileClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetProjectileClipping(checkX - 1, checkY, height) & 0x1280102) == 0;

            if (moveTypeX == 1 && moveTypeY == -1)
                return (GetProjectileClipping(x, y, height) & 0x1280183) == 0
                       && (GetProjectileClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetProjectileClipping(checkX, checkY - 1, height) & 0x1280102) == 0;

            if (moveTypeX == -1 && moveTypeY == 1)
                return (GetProjectileClipping(x, y, height) & 0x1280138) == 0
                       && (GetProjectileClipping(checkX - 1, checkY, height) & 0x1280108) == 0
                       && (GetProjectileClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            if (moveTypeX == 1 && moveTypeY == 1)
                return (GetProjectileClipping(x, y, height) & 0x12801e0) == 0
                       && (GetProjectileClipping(checkX + 1, checkY, height) & 0x1280180) == 0
                       && (GetProjectileClipping(checkX, checkY + 1, height) & 0x1280120) == 0;

            Console.WriteLine("[FATAL ERROR]: At getClipping: " + x + ", "
                              + y + ", " + height + ", " + moveTypeX + ", "
                              + moveTypeY);
            return false;
        }
        catch (Exception e)
        {
            return true;
        }
    }

    public static Objects GetObject(int id, int x, int y, int z)
    {
        var r = GetRegion(x, y);
        if (r == null)
            return null;

        foreach (var o in r._realObjects)
            if (o.Id == id)
                if (o.X == x && o.Y == y && o.Height == z)
                    return o;

        return null;
    }


    public Objects GetRealObject(Region region, int x, int y, int z, int objId)
    {
        var r = region;
        if (r == null)
            return null;

        foreach (var o in r._realObjects)
            if (o.Id == objId)
                if (o.X == x && o.Y == y && o.Height == z)
                    return o;

        return null;
    }

    public static bool ObjectExists(int id, int x, int y, int z)
    {
        var r = GetRegion(x, y);
        if (r == null)
            return false;

        foreach (var o in r._realObjects)
            if (o.Id == id)
                if (o.X == x && o.Y == y && o.Height == z)
                    return true;

        return false;
    }

    private void RemoveClip(int x, int y, int height)
    {
        var regionAbsX = (Id >> 8) * 64;
        var regionAbsY = (Id & 0xff) * 64;
        if (_clips[height] == null)
        {
            _clips[height] = new int[64][];
            for (var i = 0; i < 64; i++) _clips[height][i] = new int[64];
        }

        _clips[height][x - regionAbsX][y - regionAbsY] = 0;
    }

    public static bool CanMove(int x, int y, int z, Direction direction)
    {
        switch (direction)
        {
            case Direction.South:
                return (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case Direction.West:
                return (GetClipping(x - 1, y, z) & 0x1280108) == 0;

            case Direction.North:
                return (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            case Direction.East:
                return (GetClipping(x + 1, y, z) & 0x1280180) == 0;

            case Direction.SouthWest:
                return (GetClipping(x - 1, y - 1, z) & 0x128010e) == 0
                       && (GetClipping(x - 1, y, z) & 0x1280108) == 0
                       && (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case Direction.NorthWest:
                return (GetClipping(x - 1, y + 1, z) & 0x1280138) == 0
                       && (GetClipping(x - 1, y, z) & 0x1280108) == 0
                       && (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            case Direction.SouthEast:
                return (GetClipping(x + 1, y - 1, z) & 0x1280183) == 0
                       && (GetClipping(x + 1, y, z) & 0x1280180) == 0
                       && (GetClipping(x, y - 1, z) & 0x1280102) == 0;

            case Direction.NorthEast:
                return (GetClipping(x + 1, y + 1, z) & 0x12801e0) == 0
                       && (GetClipping(x + 1, y, z) & 0x1280180) == 0
                       && (GetClipping(x, y + 1, z) & 0x1280120) == 0;

            default:
                // throw new ArgumentException("Invalid direction: " + direction);
                Console.WriteLine("Invalid direction: " + direction);
                return false;
        }
    }

    public static bool canMove(Location start, Location end, int xLength, int yLength)
    {
        return canMove(start.X, start.Y, end.X, end.Y, start.Z, xLength, yLength);
    }

    public static bool CanShoot(int x, int y, int z, int direction)
    {
        switch (direction)
        {
            case 0:
                return !ProjectileBlockedNorthWest(x, y, z) && !ProjectileBlockedNorth(x, y, z)
                                                            && !ProjectileBlockedWest(x, y, z);
            case 1:
                return !ProjectileBlockedNorth(x, y, z);
            case 2:
                return !ProjectileBlockedNorthEast(x, y, z) && !ProjectileBlockedNorth(x, y, z)
                                                            && !ProjectileBlockedEast(x, y, z);
            case 3:
                return !ProjectileBlockedWest(x, y, z);
            case 4:
                return !ProjectileBlockedEast(x, y, z);
            case 5:
                return !ProjectileBlockedSouthWest(x, y, z) && !ProjectileBlockedSouth(x, y, z)
                                                            && !ProjectileBlockedWest(x, y, z);
            case 6:
                return !ProjectileBlockedSouth(x, y, z);
            case 7:
                return !ProjectileBlockedSouthEast(x, y, z) && !ProjectileBlockedSouth(x, y, z)
                                                            && !ProjectileBlockedEast(x, y, z);
            default:
                throw new ArgumentException("Invalid direction: " + direction);
        }
    }

    public static (int, int) MoveToAdjacency(int targetX, int targetY, int entityX, int entityY, int z)
    {
        // Calculate the horizontal and vertical distances from the entity to the target
        var dx = targetX - entityX;
        var dy = targetY - entityY;

        var newX = entityX + Math.Sign(dx);
        var newY = entityY + Math.Sign(dy);

        // If the horizontal distance is greater, move East or West towards
        // the target only if the position is valid.
        if (Math.Abs(dx) > Math.Abs(dy))
        {
            // Validate move using CanMove.
            if (CanMove(newX, entityY, z, 0)) return (newX, entityY);
        }
        else
        {
            // If the vertical distance is greater, move North or South towards 
            // the target only if the position is valid.

            // Validate move using CanMove.
            if (CanMove(entityX, newY, z, 0)) return (entityX, newY);
        }

        // If we cannot move, just return the current entity position.
        return (entityX, entityY);
    }

    public static (int, int) MoveAway(int playerX, int playerY, int entitySize, int targetX, int targetY,
        int targetSizeX, int targetSizeY, int z)
    {
        // Define the potential new positions with their respective directions.
        var possibleNewPositions = new List<(int, int, Direction)>
        {
            (playerX, playerY + 1, Direction.North), // North
            (playerX, playerY - 1, Direction.South), // South
            (playerX - 1, playerY, Direction.West), // West
            (playerX + 1, playerY, Direction.West) // East
        };

        // Shuffle the possible new positions.
        var random = new Random();
        possibleNewPositions = possibleNewPositions.OrderBy(x => random.Next()).ToList();

        foreach (var newPos in possibleNewPositions)
        {
            var newX = newPos.Item1;
            var newY = newPos.Item2;
            var direction = newPos.Item3;
            var isInTargetGrid = newX >= targetX && newX < targetX + targetSizeX &&
                                 newY >= targetY && newY < targetY + targetSizeY;

            // now pass the direction to CanMove
            var canMove = CanMove(newX, newY, z, direction);

            // Check if the new position is not in the target's grid and is not obstructed.
            if (!isInTargetGrid && canMove) return (newX, newY); // Return new valid position.
        }

        // If no valid new positions, return current position as fallback.
        return (playerX, playerY);
    }

    public static bool CanMeleeAttack(int attackerX, int attackerY, int attackerSizeX, int attackerSizeY, int targetX,
        int targetY, int targetSizeX, int targetSizeY, int z)
    {
        // Check if attacker is inside the target
        if (attackerX >= targetX && attackerX < targetX + targetSizeX &&
            attackerY >= targetY && attackerY < targetY + targetSizeY)
            return false;

        // Check if target is inside the attacker
        if (targetX >= attackerX && targetX < attackerX + attackerSizeX &&
            targetY >= attackerY && targetY < attackerY + attackerSizeY)
            return false;

        for (var i = 0; i < attackerSizeX; i++)
        for (var j = 0; j < attackerSizeY; j++)
        for (var x = 0; x < targetSizeX; x++)
        for (var y = 0; y < targetSizeY; y++)
            if (
                (targetX + x == attackerX + i && targetY + y == attackerY + j - 1 &&
                 CanMove(attackerX + i, attackerY + j, z, Direction.North)) || // north
                (targetX + x == attackerX + i && targetY + y == attackerY + j + 1 &&
                 CanMove(attackerX + i, attackerY + j, z, Direction.South)) || // south
                (targetX + x == attackerX + i - 1 && targetY + y == attackerY + j &&
                 CanMove(attackerX + i, attackerY + j, z, Direction.East)) || // east
                (targetX + x == attackerX + i + 1 && targetY + y == attackerY + j &&
                 CanMove(attackerX + i, attackerY + j, z, Direction.West)) // west
            )
                return true; // the attacker can attack the target

        return false;
    }


    public static (int, int) MoveCloserToGrid(int playerX, int playerY, int entitySize, int targetX, int targetY,
        int targetSizeX, int targetSizeY, int z)
    {
        var rand = new Random();

        var validMoves = new List<(int, int)>();

        // Possible directions, rearranged.
        // Prioritize movement in the direction of the target.
        var relativeX = playerX.CompareTo(targetX);
        var relativeY = playerY.CompareTo(targetY);
        var directions = new (int, int, int)[4];

        if (relativeX < 0) // target is east
        {
            directions[0] = (1, 0, 4); // East
            directions[3] = (-1, 0, 3); // West
        }
        else // target is west or in the same column
        {
            directions[0] = (-1, 0, 3); // West
            directions[3] = (1, 0, 4); // East
        }

        if (relativeY < 0) // target is north
        {
            directions[1] = (0, 1, 1); // North
            directions[2] = (0, -1, 6); // South
        }
        else // target is south or in the same row
        {
            directions[1] = (0, -1, 6); // South
            directions[2] = (0, 1, 1); // North
        }

        foreach (var (deltaX, deltaY, dir) in directions)
        {
            var canMove = true;

            for (var ex = 0; ex < entitySize; ex++)
            {
                for (var ey = 0; ey < entitySize; ey++)
                {
                    var newX = playerX + deltaX + ex;
                    var newY = playerY + deltaY + ey;

                    if (newX >= targetX && newX < targetX + targetSizeX && newY >= targetY &&
                        newY < targetY + targetSizeY)
                    {
                        canMove = false;
                        break;
                    }

                    switch (dir)
                    {
                        case 1:
                            canMove &= !BlockedNorth(newX, newY, z);
                            break; // North
                        case 6:
                            canMove &= !BlockedSouth(newX, newY, z);
                            break; // South
                        case 3:
                            canMove &= !BlockedWest(newX, newY, z);
                            break; // West
                        case 4:
                            canMove &= !BlockedEast(newX, newY, z);
                            break; // East
                        default:
                            canMove = false;
                            break;
                    }

                    if (!canMove)
                        break;
                }

                if (!canMove)
                    break;
            }

            if (canMove)
                validMoves.Add((playerX + deltaX, playerY + deltaY));
        }

        if (validMoves.Count == 0) return (playerX, playerY); // No valid moves; stay in the current position

        // Choose a random move from the list of valid moves
        var index = rand.Next(validMoves.Count);
        return validMoves[index];
    }

    public static bool canMove(int startX, int startY, int endX, int endY, int height, int xLength, int yLength)
    {
        var diffX = endX - startX;
        var diffY = endY - startY;
        var max = Math.Max(Math.Abs(diffX), Math.Abs(diffY));

        for (var ii = 0; ii < max; ii++)
        {
            var currentX = endX - diffX;
            var currentY = endY - diffY;
            for (var x = 0; x < xLength; x++)
            for (var y = 0; y < yLength; y++)
                if (diffX < 0 && diffY < 0)
                {
                    if ((GetClipping(currentX + x - 1,
                            currentY + y - 1, height) & 0x128010e) != 0
                        || (GetClipping(currentX + x - 1, currentY
                                                          + y, height) & 0x1280108) != 0
                        || (GetClipping(currentX + x,
                            currentY + y - 1, height) & 0x1280102) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + x + 1, currentY + y + 1,
                            height) & 0x12801e0) != 0
                        || (GetClipping(currentX + x + 1,
                            currentY + y, height) & 0x1280180) != 0
                        || (GetClipping(currentX + x,
                            currentY + y + 1, height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + x - 1, currentY + y + 1,
                            height) & 0x1280138) != 0
                        || (GetClipping(currentX + x - 1, currentY
                                                          + y, height) & 0x1280108) != 0
                        || (GetClipping(currentX + x,
                            currentY + y + 1, height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY < 0)
                {
                    if ((GetClipping(currentX + x + 1, currentY + y - 1,
                            height) & 0x1280183) != 0
                        || (GetClipping(currentX + x + 1,
                            currentY + y, height) & 0x1280180) != 0
                        || (GetClipping(currentX + x,
                            currentY + y - 1, height) & 0x1280102) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY == 0)
                {
                    if ((GetClipping(currentX + x + 1, currentY + y,
                            height) & 0x1280180) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY == 0)
                {
                    if ((GetClipping(currentX + x - 1, currentY + y, height) & 0x1280108) != 0)
                        return false;
                }
                else if (diffX == 0 && diffY > 0)
                {
                    if ((GetClipping(currentX + x, currentY + y + 1, height) & 0x1280120) != 0)
                        return false;
                }
                else if (diffX == 0 && diffY < 0  && (GetClipping(currentX + x, currentY + y - 1, height) & 0x1280102) != 0)
                {
                    return false;
                }

            if (diffX < 0)
                diffX++;
            else if (diffX > 0)
                diffX--;
            if (diffY < 0)
                diffY++;
            else if (diffY > 0)
                diffY--;
        }

        return true;
    }

    /* Correct one */
    public static bool canProjectileMove(int startX, int startY, int endX, int endY, int height, int xLength,
        int yLength)
    {
        var diffX = endX - startX;
        var diffY = endY - startY;
        // height %= 4;
        var max = Math.Max(Math.Abs(diffX), Math.Abs(diffY));
        for (var ii = 0; ii < max; ii++)
        {
            var currentX = endX - diffX;
            var currentY = endY - diffY;
            for (var i = 0; i < xLength; i++)
            for (var i2 = 0; i2 < yLength; i2++)
                if (diffX < 0 && diffY < 0)
                {
                    if ((GetProjectileClipping(currentX + i - 1, currentY + i2 - 1, height) & (UNLOADED_TILE
                            | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_EAST_BLOCKED
                            | PROJECTILE_NORTH_EAST_BLOCKED | PROJECTILE_NORTH_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i - 1, currentY + i2, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_EAST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i, currentY + i2 - 1, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_NORTH_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY > 0)
                {
                    if ((GetProjectileClipping(currentX + i + 1, currentY + i2 + 1, height) & (UNLOADED_TILE
                            | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_WEST_BLOCKED
                            | PROJECTILE_SOUTH_WEST_BLOCKED | PROJECTILE_SOUTH_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i + 1, currentY + i2, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_WEST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i, currentY + i2 + 1, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_SOUTH_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY > 0)
                {
                    if ((GetProjectileClipping(currentX + i - 1, currentY + i2 + 1, height) & (UNLOADED_TILE
                            | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_SOUTH_BLOCKED
                            | PROJECTILE_SOUTH_EAST_BLOCKED | PROJECTILE_EAST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i - 1, currentY + i2, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_EAST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i, currentY + i2 + 1, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_SOUTH_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY < 0)
                {
                    if ((GetProjectileClipping(currentX + i + 1, currentY + i2 - 1, height) & (UNLOADED_TILE
                            | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_WEST_BLOCKED
                            | PROJECTILE_NORTH_BLOCKED | PROJECTILE_NORTH_WEST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i + 1, currentY + i2, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_WEST_BLOCKED)) != 0
                        || (GetProjectileClipping(currentX + i, currentY + i2 - 1, height)
                            & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                               | PROJECTILE_NORTH_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX > 0 && diffY == 0)
                {
                    if ((GetProjectileClipping(currentX + i + 1, currentY + i2, height)
                         & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                            | PROJECTILE_WEST_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX < 0 && diffY == 0)
                {
                    if ((GetProjectileClipping(currentX + i - 1, currentY + i2, height)
                         & (UNLOADED_TILE | /* BLOCKED_TILE | */ UNKNOWN | PROJECTILE_TILE_BLOCKED
                            | PROJECTILE_EAST_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX == 0 && diffY > 0)
                {
                    if ((GetProjectileClipping(currentX + i, currentY + i2 + 1, height) & (UNLOADED_TILE
                            | /*
                               * BLOCKED_TILE |
                               */
                            UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_SOUTH_BLOCKED)) != 0)
                        return false;
                }
                else if (diffX == 0 && diffY < 0)
                {
                    if ((GetProjectileClipping(currentX + i, currentY + i2 - 1, height) & (UNLOADED_TILE
                            | /*
                               * BLOCKED_TILE |
                               */
                            UNKNOWN | PROJECTILE_TILE_BLOCKED | PROJECTILE_NORTH_BLOCKED)) != 0)
                        return false;
                }

            if (diffX < 0)
                diffX++;
            else if (diffX > 0) diffX--;

            if (diffY < 0)
                diffY++; // change
            else if (diffY > 0) diffY--;
        }

        return true;
    }

    public static bool canInteract(int dstX, int dstY, int absX, int absY, int curX, int curY, int sizeX, int sizeY, int walkToData)
    {
        // if ((walkToData & 0x80000000) != 0)
        //     if (curX == dstX && curY == dstY)
        //         return false;

        var maxX = dstX + sizeX - 1;
        var maxY = dstY + sizeY - 1;

        var region = GetRegion(absX, absY);
        var clipping = region.GetClip(absX, absY, 0);

        if (curX >= dstX && maxX >= curX && dstY <= curY && maxY >= curY) return true;

        if (curX == dstX - 1 && curY >= dstY && curY <= maxY && (clipping & 8) == 0 &&
            (walkToData & 8) == 0) return true;

        if (curX == maxX + 1 && curY >= dstY && curY <= maxY && (clipping & 0x80) == 0 &&
            (walkToData & 2) == 0) return true;

        return (curY == dstY - 1 && curX >= dstX && curX <= maxX && (clipping & 2) == 0 && (walkToData & 4) == 0)
               || (curY == maxY + 1 && curX >= dstX && curX <= maxX && (clipping & 0x20) == 0
                   && (walkToData & 1) == 0);
    }

    // public static bool canInteract(int dstX, int dstY, int curX, int curY, int entitySizeX, int entitySizeY,
    //     int targetSizeX, int targetSizeY, int walkToData)
    // {
    //     var maxX = dstX + targetSizeX - 1;
    //     var maxY = dstY + targetSizeY - 1;
    //
    //     var region = GetRegion(curX, curY);
    //     var clipping = region.GetClip(curX, curY, 0);
    //
    //     for (var x = 0; x < entitySizeX; x++)
    //     for (var y = 0; y < entitySizeY; y++)
    //     {
    //         var entityCurX = curX + x;
    //         var entityCurY = curY + y;
    //
    //         if (entityCurX >= dstX && maxX >= entityCurX && dstY <= entityCurY && maxY >= entityCurY) return true;
    //
    //         if (entityCurX == dstX - 1 && entityCurY >= dstY && entityCurY <= maxY && (clipping & 8) == 0 &&
    //             (walkToData & 8) == 0) return true;
    //
    //         if (entityCurX == maxX + 1 && entityCurY >= dstY && entityCurY <= maxY && (clipping & 0x80) == 0 &&
    //             (walkToData & 2) == 0) return true;
    //
    //         if ((entityCurY == dstY - 1 && entityCurX >= dstX && entityCurX <= maxX && (clipping & 2) == 0 &&
    //              (walkToData & 4) == 0) ||
    //             (entityCurY == maxY + 1 && entityCurX >= dstX && entityCurX <= maxX && (clipping & 0x20) == 0 &&
    //              (walkToData & 1) == 0)) return true;
    //     }
    //
    //     return false;
    // }

    public static bool BlockedShot(int x, int y, int height, int moveTypeX, int moveTypeY)
    {
        try
        {
            if (height > 3)
                height = 0;

            var checkX = x + moveTypeX;
            var checkY = y + moveTypeY;

            if (moveTypeX == -1 && moveTypeY == 0)
                return ProjectileBlockedWest(x + 1, y, height);
            if (moveTypeX == 1 && moveTypeY == 0)
                return ProjectileBlockedEast(x - 1, y, height);
            if (moveTypeX == 0 && moveTypeY == -1)
                return ProjectileBlockedSouth(x, y + 1, height);
            if (moveTypeX == 0 && moveTypeY == 1)
                return ProjectileBlockedNorth(x, y - 1, height);
            if (moveTypeX == -1 && moveTypeY == -1)
                return BlockedRangeSouthWest(x + 1, y + 1, height);
            if (moveTypeX == 1 && moveTypeY == -1)
                return BlockedRangeSouthEast(x - 1, y + 1, height);
            if (moveTypeX == -1 && moveTypeY == 1)
                return BlockedRangeNorthWest(x + 1, y - 1, height);
            if (moveTypeX == 1 && moveTypeY == 1)
                return BlockedRangeNorthEast(x - 1, y - 1, height);
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }

    public static bool BlockedRangeWest(int x, int y, int z)
    {
        var type = GetClipping(x - 1, y, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x1280108) == 0 ||
                ((type - 0x0002000) & 0x1280108) == 256 ||
                ((type - 0x0002000) & 0x1280108) == 2 ||
                ((type - 0x0002000) & 0x1280108) == 32 ||
                ((type - 0x0002000) & 0x1280108) == 8 ||
                GetClipping(x - 1, y, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x - 1, y, z) & 0x1280108) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeEast(int x, int y, int z)
    {
        var type = GetClipping(x + 1, y, z);
        var isSet = (type & 0x0002000) != 0;

        if (type - 0x0002000 == 224)
            return false;

        if (isSet)
        {
            if (((GetClipping(x + 1, y, z) - 0x0002000) & 0x1280180) == 0 ||
                ((GetClipping(x + 1, y, z) - 0x0002000) & 0x1280180) == 128 ||
                ((GetClipping(x + 1, y, z) - 0x0002000) & 0x1280180) == 256 ||
                ((GetClipping(x + 1, y, z) - 0x0002000) & 0x1280180) == 32 ||
                GetClipping(x + 1, y, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x + 1, y, z) & 0x1280180) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeSouth(int x, int y, int z)
    {
        var type = GetClipping(x, y - 1, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x1280102) == 0 ||
                ((type - 0x0002000) & 0x1280102) == 256 ||
                ((type - 0x0002000) & 0x1280102) == 2 ||
                ((type - 0x0002000) & 0x1280102) == 32 ||
                GetClipping(x, y - 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x, y - 1, z) & 0x1280102) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeNorth(int x, int y, int z)
    {
        var clip = GetClipping(x, y + 1, z);
        var hex = clip.ToString("X");
        var type = GetClipping(x, y + 1, z);

        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((GetClipping(x, y + 1, z) - 0x0002000) & 0x1280120) == 0 ||
                ((GetClipping(x, y + 1, z) - 0x0002000) & 0x1280120) == 288 ||
                ((GetClipping(x, y + 1, z) - 0x0002000) & 0x1280120) == 256 ||
                ((GetClipping(x, y + 1, z) - 0x0002000) & 0x1280120) == 32 ||
                GetClipping(x, y + 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x, y + 1, z) & 0x1280120) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeSouthWest(int x, int y, int z)
    {
        var type = GetClipping(x - 1, y - 1, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x128010e) == 0 ||
                ((type - 0x0002000) & 0x128010e) == 8 ||
                ((type - 0x0002000) & 0x128010e) == 256 ||
                ((type - 0x0002000) & 0x128010e) == 2 ||
                ((type - 0x0002000) & 0x128010e) == 32 ||
                GetClipping(x - 1, y - 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x - 1, y - 1, z) & 0x128010e) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeSouthEast(int x, int y, int z)
    {
        var type = GetClipping(x + 1, y - 1, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x1280183) == 0 ||
                ((type - 0x0002000) & 0x1280183) == 256 ||
                ((type - 0x0002000) & 0x1280183) == 128 ||
                ((type - 0x0002000) & 0x1280183) == 2 ||
                ((type - 0x0002000) & 0x1280183) == 32 ||
                GetClipping(x + 1, y - 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x + 1, y - 1, z) & 0x1280183) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeNorthWest(int x, int y, int z)
    {
        var type = GetClipping(x - 1, y + 1, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x1280138) == 0 ||
                ((type - 0x0002000) & 0x1280138) == 8 ||
                ((type - 0x0002000) & 0x1280138) == 256 ||
                ((type - 0x0002000) & 0x1280138) == 2 ||
                ((type - 0x0002000) & 0x1280138) == 32 ||
                GetClipping(x - 1, y + 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x - 1, y + 1, z) & 0x1280138) == 0)
            return true;
        return false;
    }

    public static bool BlockedRangeNorthEast(int x, int y, int z)
    {
        var type = GetClipping(x + 1, y + 1, z);
        var isSet = (type & 0x0002000) != 0;

        if (isSet)
        {
            if (((type - 0x0002000) & 0x12801e0) == 0 ||
                ((type - 0x0002000) & 0x12801e0) == 256 ||
                ((type - 0x0002000) & 0x12801e0) == 128 ||
                ((type - 0x0002000) & 0x12801e0) == 2 ||
                ((type - 0x0002000) & 0x12801e0) == 32 ||
                GetClipping(x + 1, y + 1, z) == 0)
                return true;

            return false;
        }

        if ((GetClipping(x + 1, y + 1, z) & 0x12801e0) == 0)
            return true;
        return false;
    }
}