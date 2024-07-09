using RuneRealm.Entities;

namespace RuneRealm.Events;

public class CombatEvent : RSEvent
{
    private Player player;
    private readonly Guid _guid;
    private int cycleCount;

    public CombatEvent(Player player, Guid guid)
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

        if (cycleCount % 3 == 0)
        {
            player.PacketBuilder.SendMessage("Combat Event..");
            player.SetCurrentAnimation(422);
        }
        else
        {
            player.PacketBuilder.SendMessage("Combat Event Delay..");
        }

        cycleCount++;
    }

    public override void Stop()
    {
        player.PacketBuilder.SendMessage("CombatEvent Finished!");
        player.SetCurrentAnimation(-1);
    }
}