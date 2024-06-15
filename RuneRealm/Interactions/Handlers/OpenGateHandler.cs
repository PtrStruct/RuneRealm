﻿using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class OpenGateHandler : IInteractionHandler
{
    public void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        if (interactingObject.Id == 5492)
        {
            player.SetCurrentAnimation(827);
        }
        
        player.PacketBuilder.SendMessage("Open gate!");
        player.InteractingWorldObject = null;
    }
}