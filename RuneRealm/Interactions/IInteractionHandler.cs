using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public interface IInteractionHandler
{
    void HandleInteraction(Player player, InteractingObjectModel interactingObject);
}
