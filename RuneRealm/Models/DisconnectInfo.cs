using RuneRealm.Entities;

namespace RuneRealm.Models;

public class DisconnectInfo
{
    public DisconnectInfo(Player player, string reason)
    {
        Player = player;
        Reason = reason;
    }

    public Player Player { get; }
    public string Reason { get; }
}