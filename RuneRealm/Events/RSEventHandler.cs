namespace RuneRealm.Events;

public class RSEventHandler
{
    private static readonly Lazy<RSEventHandler> lazyInstance = new Lazy<RSEventHandler>(() => new RSEventHandler());
    public static RSEventHandler Instance => lazyInstance.Value;

    private readonly List<RSEventContainer> events;

    private RSEventHandler()
    {
        events = new List<RSEventContainer>();
    }

    public void AddEvent(object owner, RSEvent rsEvent, int interval)
    {
        events.Add(new RSEventContainer(-1, owner, rsEvent, interval));
    }

    public void Process()
    {
        var eventsCopy = new List<RSEventContainer>(events);
        var remove = new List<RSEventContainer>();

        foreach (var c in eventsCopy)
        {
            if (c.NeedsExecution() && c.IsRunning)
            {
                c.Execute();
                if (!c.IsRunning)
                {
                    remove.Add(c);
                }
            }
        }

        foreach (var c in remove)
        {
            events.Remove(c);
        }
    }
}
