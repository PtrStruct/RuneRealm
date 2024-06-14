namespace RuneRealm.Interactions;

public static class InteractionTypeMapping
{
    public static readonly Dictionary<int, InteractionType> TypeMap = new()
    {
        { 1276, InteractionType.WOODCUTTING },
        { 1281, InteractionType.WOODCUTTING },
        { 2092, InteractionType.MINING },     
        { 1534, InteractionType.GATE }
    };
}
