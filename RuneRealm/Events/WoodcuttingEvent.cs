using RuneRealm.Entities;

namespace RuneRealm.Events;

public class WoodcuttingEvent : RSEvent
{
    // private readonly Tree tree;
    private readonly Player _player;
    private readonly Guid _guid;

    public WoodcuttingEvent(Player player,  Guid guid) //Player player, Tree tree, int cyclesToChop
    {
        _player = player;
        // tree = tree;
        _guid = guid;
    }

    public override void Execute(RSEventContainer container)
    {
        
        var player = (Player)container.Owner;
        
        var valid = player.CheckTask(_guid);
        if (!valid)
        {
            container.Stop();
            return;
        }

        player.PacketBuilder.SendMessage("Woodcutting..");
        
        if (SkillCheck(1, 5, 0))
        {
            player.PacketBuilder.SendMessage("You received x1 log.");
            container.Stop();
            return;
        }

        _player.SetCurrentAnimation(875);
    }

    public override void Stop()
    {
        _player.SetCurrentAnimation(-1);
    }
    
    public bool SkillCheck(int level, int levelRequired, int itemBonus)
    {
        double chance = 0.0;
        double baseChance = Math.Pow(10d - levelRequired / 10d, 2d) / 2d;
        chance = baseChance + ((level - levelRequired) / 2d) + (itemBonus / 10d);
        return chance >= (new Random().NextDouble() * 100.0);
    }
    
}