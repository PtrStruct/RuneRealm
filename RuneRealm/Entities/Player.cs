using RuneRealm.Constants;
using RuneRealm.Environment;
using RuneRealm.Movement;
using RuneRealm.Network;
using RuneRealm.Network.Packet;

namespace RuneRealm.Entities;

public class Player : Entity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public PacketBuilder PacketBuilder { get; set; }
    public PlayerSession Session { get; set; } = new();
    public PlayerUpdateFlags Flags { get; set; }
    public bool NeedsPositionUpdate { get; set; }
    public MovementHandler MovementHandler { get; set; }
    public List<Player> LocalPlayers { get; set; } = new();

    public Player()
    {
        PacketBuilder = new PacketBuilder(this);
        MovementHandler = new MovementHandler(this);
        Location = new Location(3200, 3200, 0);
    }
}