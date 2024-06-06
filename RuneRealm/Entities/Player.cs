using RuneRealm.Constants;
using RuneRealm.Environment;
using RuneRealm.Managers;
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
    public int Gender { get; set; }
    public int HeadIcon { get; set; }
    public AppearanceManager AppearanceManager { get; set; } = new();
    public EquipmentManager EquipmentManager { get; set; }
    public ColorManager ColorManager { get; set; } = new();
    public AnimationManager AnimationManager { get; set; } = new();
    
    public Player()
    {
        PacketBuilder = new PacketBuilder(this);
        MovementHandler = new MovementHandler(this);
        EquipmentManager = new EquipmentManager(this);
        Location = new Location(3200, 3200, 0);
    }

    public override void Reset()
    {
        NeedsPositionUpdate = true;
        Flags |= PlayerUpdateFlags.Appearance;
    }
}