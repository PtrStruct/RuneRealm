using RuneRealm.Entities;

namespace RuneRealm.Events;

public class WoodcuttingEvent : RSEvent
{
    // private readonly Tree tree;
    private readonly Player _player;
    private readonly int _cyclesToChop;
    private readonly Guid _guid;

    public WoodcuttingEvent(Player player, int cyclesToChop, Guid guid) //Player player, Tree tree, int cyclesToChop
    {
        _player = player;
        // tree = tree;
        _cyclesToChop = cyclesToChop;
        _guid = guid;
    }

    public override void Execute(RSEventContainer container)
    {
        // if (!player.HasAxe() || player.WoodcuttingLevel < tree.RequiredLevel)
        // {
        //     container.Stop();
        //     return;
        // }

        var player = (Player)container.Owner;
        var valid = player.CheckTask(_guid);

        if (!valid)
        {
            container.Stop();
            return;
        }
        
        _player.SetCurrentAnimation(875);
        
         if (++container.Interval >= _cyclesToChop)
         {
             _player.PacketBuilder.SendMessage("Finished woodcutting.");
             // player.AddExperience(tree.Experience);
             // player.AddLog(tree.LogType);
             // tree.ChopDown();
             container.Stop();
         }
    }

    public override void Stop()
    {
        _player.SetCurrentAnimation(-1);
    }
}
