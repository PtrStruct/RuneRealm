using RuneRealm.Entities;
using RuneRealm.Models;
using RuneRealm.Movement;

namespace RuneRealm.Events;

public class LadderClimbEvent : RSEvent
{
    private readonly int _heightChange;
    private readonly InteractingObjectModel _item;
    private readonly Guid _guid;

    public LadderClimbEvent(int heightChange, InteractingObjectModel item, Guid guid)
    {
        _heightChange = heightChange;
        _item = item;
        _guid = guid;
    }

    public override void Execute(RSEventContainer container)
    {
        var player = (Player)container.Owner;

        var valid = player.CheckTask(_guid);
        
        if (!valid)
            container.Stop();

        if (MovementHelper.EuclideanDistance(player.Location.X, player.Location.Y, _item.X, _item.Y) > 1)
            return;

        //player.Location.Z += _heightChange;          
        player.SetCurrentAnimation(828);
        container.Stop();
    }

    public override void Stop()
    {
        // var player = (Player)container.Owner;
        // player.SetMovementBlocked(false);
        // player.ResetInteractingWorldObject();
    }
}