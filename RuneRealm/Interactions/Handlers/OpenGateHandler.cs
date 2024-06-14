using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class OpenGateHandler : IInteractionHandler
{
    public void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Open gate!");
        player.InteractingWorldObject = null;
    }
}