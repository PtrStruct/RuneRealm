using System.Numerics;
using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class WoodcuttingHandler : IInteractionHandler
{
    private int i = 0;

    public void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        i++;
        player.PacketBuilder.SendMessage("Woodcutting!");
        
        player.SetFacingDirection(new Vector2(interactingObject.X, interactingObject.Y));
        
        if (player.CurrentAnimation != 875)
        {
            player.SetCurrentAnimation(875);
        }

        if (i == 10)
        {
            player.InteractingWorldObject = null;
            player.SetCurrentAnimation(-1);
            Console.WriteLine("Finished woodcutting!");
        }
    }
}