using RuneRealm.Entities;
using RuneRealm.Environment;
using RuneRealm.Events;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class WoodcuttingHandler : InteractionHandler
{
    public override void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        /* Guard clauses */
        
        /* Distance check */
        if (!CanInteract(player, null))
            return;
        
        // if (!player.HasAxe() || player.WoodcuttingLevel < tree.RequiredLevel)
        // {
        //     container.Stop();
        //     return;
        // }

        var hasRequiredLevel = true;
        if (!hasRequiredLevel)
        {
            player.ResetInteractingWorldObject();
            return;
        }
        
        var guid = player.StartNewTask();
        World.RSEventHandler.AddEvent(player, new WoodcuttingEvent(player, guid), 4);
        player.ResetInteractingWorldObject();
        player.SetCurrentAnimation(875);
    }

    private bool CanInteract(Player player, InteractingObjectModel interactingObjectModel)
    {
        var hasInteractingObject = player.InteractingWorldObject != null;

        return hasInteractingObject && Region.canInteract(
            player.InteractingWorldObject.X,
            player.InteractingWorldObject.Y,
            player.Location.X,
            player.Location.Y,
            1,
            1,
            player.InteractingWorldObject.Width,
            player.InteractingWorldObject.Height, 0);
    }
}