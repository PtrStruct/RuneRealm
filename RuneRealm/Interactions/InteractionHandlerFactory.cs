﻿namespace RuneRealm.Interactions;

public class InteractionHandlerFactory
{
    public static IInteractionHandler GetHandler(InteractionType interactionType)
    {
        return interactionType switch
        {
            InteractionType.WOODCUTTING => new WoodcuttingHandler(),
            InteractionType.MINING => new MiningHandler(),
            InteractionType.GATE => new OpenGateHandler(),
            InteractionType.BANK => new BankHandler(),
            InteractionType.THIEVING_STALL => new BankHandler(),
            _ => null
        };
    }
}