using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class MiningHandler : IInteractionHandler
{
    public void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Start mining!");
        player.InteractingWorldObject = null;
    }
}