using System.Net.Sockets;
using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Environment;
using RuneRealm.Models;

namespace RuneRealm.Network;

public class ClientManager
{
    public static Player InitializeClient(TcpClient tcpClient)
    {
        var player = new Player();
        player.Session.Initialize(tcpClient);
        return player;
    }
    
    public static void AssignAvailablePlayerSlot(Player player)
    {
        if (World.Players.Count >= ServerConfig.MAX_PLAYERS)
        {
            Console.WriteLine($"Server is full! Disconnecting {player.Session.Socket.Client.RemoteEndPoint}.");
            player.Session.Disconnect(new DisconnectInfo(player, "Server is full!"));
            throw new Exception("Server is full!");
        }

        World.Players.Add(player);
        player.Session.Index = World.Players.Count;
        Console.WriteLine($"Incoming connection has been assigned to player {player.Username}!");
    }

    public static void Login(Player player)
    {
        player.PacketBuilder.BuildNewBuildAreaPacket();
        player.NeedsPositionUpdate = true;
        player.Flags |= PlayerUpdateFlags.Appearance;
    }
    
}