using RuneRealm.Environment;
using RuneRealm.Network;
using RuneRealm.Network.Packet;

namespace RuneRealm.Entities;

public class Player : Entity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Location Location { get; set; }
    public PacketBuilder PacketBuilder { get; set; }
    public PlayerSession Session { get; set; } = new();

    public Player()
    {
        PacketBuilder = new PacketBuilder(this);
        Location = new Location(3200, 3200, 0);
    }
}