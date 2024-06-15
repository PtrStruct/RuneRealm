using RuneRealm.Entities;
using RuneRealm.Environment;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class WoodcuttingHandler : InteractionHandler
{
    private int i = 0;

    public override void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        if (!CanInteract(player, null))
            return;

        i++;
        player.PacketBuilder.SendMessage("Woodcutting!");

        player.SetCurrentAnimation(875);

        if (i == 10)
        {
            player.ResetInteractingWorldObject();
            player.SetCurrentAnimation(-1);

            Console.WriteLine("Finished woodcutting!");
        }
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