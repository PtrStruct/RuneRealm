using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class ThievingStall : InteractionHandler
{
    public override void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Steal from stall!");
        player.InteractingWorldObject = null;
    }
}