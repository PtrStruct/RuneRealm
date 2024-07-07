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
        
        cycleCount++;

        if (cycleCount > 5)
        {
            container.Stop();
            return;
        }

        player.PacketBuilder.SendMessage("Event..");
        player.SetCurrentAnimation(875);
    }

    public override void Stop()
    {
        player.PacketBuilder.SendMessage("Finished!");
        player.SetCurrentAnimation(-1);
    }
}