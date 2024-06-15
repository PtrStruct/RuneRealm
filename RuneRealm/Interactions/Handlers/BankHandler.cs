using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class BankHandler : IInteractionHandler
{
    public void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Open Bank!");
        player.ResetInteractingWorldObject();
    }
}