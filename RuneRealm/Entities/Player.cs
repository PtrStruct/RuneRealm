using RuneRealm.Constants;
using RuneRealm.Environment;
using RuneRealm.Managers;
using RuneRealm.Movement;
using RuneRealm.Network;
using RuneRealm.Network.Packets;
using RuneRealm.Tasks;

namespace RuneRealm.Entities;

public class Player : Entity
{
    public string Username { get; set; }
    public string Password { get; set; }
    public PacketBuilder PacketBuilder { get; set; }
    public PlayerSession Session { get; set; }
    public PlayerUpdateFlags Flags { get; set; }
    public bool NeedsPositionUpdate { get; set; }
    public override MovementHandler MovementHandler { get; set; }
    public override RSTaskScheduler TaskScheduler { get; set; } = new();
    public List<Player> LocalPlayers { get; set; } = new();
    public int Gender { get; set; }
    public int HeadIcon { get; set; }
    public AppearanceManager AppearanceManager { get; set; } = new();
    public EquipmentManager EquipmentManager { get; set; }
    public ColorManager ColorManager { get; set; } = new();
    public AnimationManager AnimationManager { get; set; } = new();
    public InventoryManager InventoryManager { get; set; }
    public override Location Location { get; set; }

    public Player()
    {
        PacketBuilder = new PacketBuilder(this);
        MovementHandler = new MovementHandler(this);
        EquipmentManager = new EquipmentManager(this);
        InventoryManager = new InventoryManager(this);
        Session = new PlayerSession(this);
        Location = new Location(3200, 3200, 0);
    }

    public override void Reset()
    {
        NeedsPositionUpdate = false;
        Flags |= PlayerUpdateFlags.None;

        MovementHandler.PrimaryDirection = -1;
        MovementHandler.SecondaryDirection = -1;
        MovementHandler.IsRunning = false;
        MovementHandler.IsWalking = false;
        MovementHandler.DiscardMovementQueue = false;
    }
}