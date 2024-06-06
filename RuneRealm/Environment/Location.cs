using System.Text.Json.Serialization;

namespace RuneRealm.Environment;

public class Location
{
    public Location(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
        Update();
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }

    [JsonIgnore] public int CenterChunkX { get; set; }
    [JsonIgnore] public int CenterChunkY { get; set; }
    [JsonIgnore] public int RegionId { get; set; }
    [JsonIgnore] public int OffsetChunkX { get; set; }
    [JsonIgnore] public int OffsetChunkY { get; set; }
    [JsonIgnore] public int BuildAreaStartX { get; set; }
    [JsonIgnore] public int BuildAreaStartY { get; set; }
    [JsonIgnore] public int PositionRelativeToOffsetChunkX => X - OffsetChunkX * 8;
    [JsonIgnore] public int PositionRelativeToOffsetChunkY => Y - OffsetChunkY * 8;

    [JsonIgnore]
    public bool ShouldGenerateNewBuildArea => PositionRelativeToOffsetChunkX < 16 ||
                                              PositionRelativeToOffsetChunkX >= 88 ||
                                              PositionRelativeToOffsetChunkY < 16 ||
                                              PositionRelativeToOffsetChunkY >= 88;

    public void Update()
    {
        if (ShouldGenerateNewBuildArea)
        {
            CenterChunkX = X >> 3;
            CenterChunkY = Y >> 3;
            RegionId = (((X >> 6) << 8) & 0xFF00) | ((Y >> 6) & 0xFF);
            OffsetChunkX = CenterChunkX - 6;
            OffsetChunkY = CenterChunkY - 6;
            BuildAreaStartX = OffsetChunkX << 3;
            BuildAreaStartY = OffsetChunkY << 3;
        }
    }

    public bool IsWithinArea(Location entityLocation)
    {
        var delta = Delta(this, entityLocation);
        return delta.X <= 14 && delta.X >= -15 && delta.Y <= 14 && delta.Y >= -15 && entityLocation.Z == Z;
    }

    public static Location Delta(Location a, Location b)
    {
        return new Location(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
    }

    internal void Move(int amountX, int amountY)
    {
        X += amountX;
        Y += amountY;
    }

    public List<string> ToStringParts()
    {
        var messageParts = new List<string>
        {
            $"X: {X} Y: {Y}",
            $"CenterChunkX: {CenterChunkX} CenterChunkY: {CenterChunkY}",
            $"RegionId: {RegionId} OffsetChunkX: {OffsetChunkX} OffsetChunkY: {OffsetChunkY}",
            $"BuildAreaStartX: {BuildAreaStartX} BuildAreaStartY: {BuildAreaStartY}",
            $"PositionRelativeToOffsetChunkX: {PositionRelativeToOffsetChunkX} PositionRelativeToOffsetChunkY: {PositionRelativeToOffsetChunkY}",
            $"IsOutside: {ShouldGenerateNewBuildArea}"
        };

        return messageParts;
    }
}