using System.Net;
using System.Net.Sockets;
using RuneRealm.Constants;
using RuneRealm.Entities;
using RuneRealm.Environment;
using RuneRealm.Models;

namespace RuneRealm.Network;

public class ConnectionManager
{
    private const int MaxClientsPerCycle = 10;
    private static TcpListener _tcpListener;

    public static void Initialize()
    {
        _tcpListener = new TcpListener(IPAddress.Any, ServerConfig.PORT);
        _tcpListener.Start(10);
    }

    public static void AcceptClients()
    {
        for (var i = 0; i < MaxClientsPerCycle; i++)
        {
            if (!_tcpListener.Pending())
                continue;

            var tcpClient = _tcpListener.AcceptTcpClient();
            Console.WriteLine(
                $"Incoming Connection From: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()}");
            try
            {
                if (World.Players.Count >= ServerConfig.MAX_PLAYERS)
                    return;

                var player = ClientManager.InitializeClient(tcpClient);

                if (player.Session.Available() < 2)
                {
                    RejectLogin(player);
                    return;
                }

                if (LoginManager.Handshake(player))
                {
                    ClientManager.AssignAvailablePlayerSlot(player);
                    ClientManager.Login(player);
                }
                else
                {
                    player.Session.Disconnect(new DisconnectInfo(player, "Invalid handshake!"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"E: {e} - MSG: {e.Message}");
            }
        }
    }

    private static void RejectLogin(Player player)
    {
        player.Session.Close();
        World.Players.Remove(player);
        Console.WriteLine("Connection Rejected.");
    }
}