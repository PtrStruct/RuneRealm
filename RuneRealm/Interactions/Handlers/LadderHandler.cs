using RuneRealm.Entities;
using RuneRealm.Events;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class LadderHandler : InteractionHandler
{
    public override void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Climb Ladder!");
        
        player.ResetInteractingWorldObject();

        var guid = player.StartNewTask();
        
        var ladderClimbEvent = new LadderClimbEvent(1, interactingObject, guid);
        RSEventHandler.Instance.AddEvent(player, ladderClimbEvent, 2); 
    }
}