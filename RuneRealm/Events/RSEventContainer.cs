namespace RuneRealm.Events;

public class RSEventContainer
{
    public object Owner { get; }
    public bool IsRunning { get; private set; }
    public int Interval { get; set; }
    public RSEvent Event { get; }
    
    private int _cyclesPassed;
    public int EventId { get; }

    public RSEventContainer(int eventId, object owner, RSEvent rsEvent, int interval)
    {
        EventId = eventId;
        Owner = owner;
        Event = rsEvent;
        IsRunning = true;
        _cyclesPassed = 0;
        Interval = interval;
    }

    public void Execute()
    {
        Event.Execute(this);
    }

    public void Stop()
    {
        IsRunning = false;
        Event.Stop();
    }

    public bool NeedsExecution()
    {
        if (!IsRunning) return false;
        
        if (++_cyclesPassed >= Interval)
        {
            _cyclesPassed = 0;
            return true;
        }
        return false;
    }
}