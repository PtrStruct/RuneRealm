namespace RuneRealm.Events;

public abstract class RSEvent
{
    public abstract void Execute(RSEventContainer container);
    public abstract void Stop();
}