using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public abstract class InteractionHandler
{
    public abstract void HandleInteraction(Player player, InteractingObjectModel interactingObject);
}
