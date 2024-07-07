namespace RuneRealm.Interactions;

public class InteractionHandlerFactory
{
    public static InteractionHandler GetHandler(InteractionType interactionType)
    {
        return interactionType switch
        {
            InteractionType.WOODCUTTING => new WoodcuttingHandler(),
            InteractionType.MINING => new MiningHandler(),
            InteractionType.LADDER => new LadderHandler(),
            InteractionType.GATE => new OpenGateHandler(),
            InteractionType.BANK => new BankHandler(),
            InteractionType.THIEVING_STALL => new NoOpHandler(),
            _ => new NoOpHandler()
        };
    }
}