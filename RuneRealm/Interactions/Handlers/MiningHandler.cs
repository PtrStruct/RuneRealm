﻿using RuneRealm.Entities;
using RuneRealm.Models;

namespace RuneRealm.Interactions;

public class MiningHandler : InteractionHandler
{
    public override void HandleInteraction(Player player, InteractingObjectModel interactingObject)
    {
        player.PacketBuilder.SendMessage("Start mining!");
        player.InteractingWorldObject = null;
    }
}