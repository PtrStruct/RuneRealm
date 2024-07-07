using RuneRealm.Entities;

namespace RuneRealm.Events;

public class MessageEvent : RSEvent
{
    private Player player;
    private readonly Guid _guid;
    private int cycleCount;

    public MessageEvent(Player player, Guid guid)
    {
        this.player = player;
        _guid = guid;
        cycleCount = 0;
    }

    public override void Execute(RSEventContainer container)
    {
        var valid = player.CheckTask(_guid);
        if (!valid)
        {
            container.Stop();
            return;
        }
        
        // cycleCount++;

        if (SkillCheck(10, 1, 3))
        {
            player.PacketBuilder.SendMessage("You received x1 log.");
            container.Stop();
            return;
        }
        // if (cycleCount > 3)
        // {
        //     container.Stop();
        //     return;
        // }

        player.PacketBuilder.SendMessage("Event..");
        player.SetCurrentAnimation(875);
    }

    public override void Stop()
    {
        player.PacketBuilder.SendMessage("Finished!");
        player.SetCurrentAnimation(-1);
    }
    
    public bool SkillCheck(int level, int levelRequired, int itemBonus)
    {
        double chance = 0.0;
        double baseChance = Math.Pow(10d - levelRequired / 10d, 2d) / 2d;
        chance = baseChance + ((level - levelRequired) / 2d) + (itemBonus / 10d);
        return chance >= (new Random().NextDouble() * 100.0);
    }
}