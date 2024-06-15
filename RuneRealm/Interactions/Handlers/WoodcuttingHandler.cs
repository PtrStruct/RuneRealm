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
        
        player.SetCurrentAnimation(875);

        if (i == 10)
        {
            player.ResetInteractingWorldObject();
            player.SetCurrentAnimation(-1);
            
            Console.WriteLine("Finished woodcutting!");
        }
    }
}