using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Managers;

namespace RuneRealm.Environment;

public class World
{
    public static List<Player> Players { get; set; } = new ();

    public static void Process()
    {
        FetchData();
        ProcessPackets();
        
        PlayerUpdateManager.Update();
        
        FlushAllPlayers();
    }
    
    private static void FetchData()
    {
        foreach (var player in Players)
            for (var i = 0; i < ServerConfig.PACKET_FETCH_LIMIT; i++)
                player.Session.Fetch();
    }

    private static void ProcessPackets()
    {
        foreach (var player in Players) player.Session.PacketStore.ProcessPackets();
    }
    
    private static void FlushAllPlayers()
    {
        foreach (var player in Players.ToList())
            player.Session.Flush();
    }
}