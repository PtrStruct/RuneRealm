using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Managers;

namespace RuneRealm.Environment;

public class World
{
    public static List<Player> Players { get; set; } = new();

    public static void Process()
    {
        FetchData();
        ProcessPackets();

        foreach (var player in Players)
        {
            if (player.InteractingWorldObject != null && player.InteractionHandler != null)
            {
                var canInteract = Region.canInteract(
                    player.InteractingWorldObject.X,
                    player.InteractingWorldObject.Y,
                    player.Location.X,
                    player.Location.Y,
                    1,
                    1,
                    player.InteractingWorldObject.Width,
                    player.InteractingWorldObject.Height, 0);

                player.PacketBuilder.SendMessage($"CanInteract: {canInteract}");

                if (canInteract)
                {
                    player.InteractionHandler.HandleInteraction(player, player.InteractingWorldObject);
                }
            }
        }
        
        foreach (var player in Players)
            player.TaskScheduler.ProcessTasks();
        
        foreach (var player in Players)
            player.MovementHandler.Process();
        
        


        PlayerUpdateManager.Update();


        FlushAllPlayers();
        ResetPlayers();
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

    private static void ResetPlayers()
    {
        foreach (var player in Players)
        {
            player.Reset();
        }
    }
}