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
        
        player.PacketBuilder.SendPlayerStatus();
        player.PacketBuilder.SendSidebarInterface(0, 2423);
        player.PacketBuilder.SendSidebarInterface(1, 3917);
        player.PacketBuilder.SendSidebarInterface(2, 638);
        player.PacketBuilder.SendSidebarInterface(3, 3213);
        player.PacketBuilder.SendSidebarInterface(4, 1644);
        player.PacketBuilder.SendSidebarInterface(5, 5608);
        player.PacketBuilder.SendSidebarInterface(6, 1151);
        player.PacketBuilder.SendSidebarInterface(8, 5065);
        player.PacketBuilder.SendSidebarInterface(9, 5715);
        player.PacketBuilder.SendSidebarInterface(10, 2449);
        player.PacketBuilder.SendSidebarInterface(11, 4445);
        player.PacketBuilder.SendSidebarInterface(12, 147);
        player.PacketBuilder.SendSidebarInterface(13, 6299);
        
        player.InventoryManager.Refresh();
        player.EquipmentManager.Refresh();
    }
    
}