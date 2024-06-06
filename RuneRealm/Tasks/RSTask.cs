namespace RuneRealm.Tasks;

public class RSTask
{
    public QueueType Type { get; private set; }
    public int Delay { get; private set; }
    public Action Action { get; private set; }
    public Func<bool> Condition { get; private set; }

    public RSTask(QueueType type, int delay, Action action, Func<bool> condition = null)
    {
        Type = type;
        Delay = delay;
        Action = action;
        Condition = condition ?? (() => true); // Default condition is always true
    }

    public bool IsReadyToExecute()
    {
        return Delay <= 0 && Condition();
    }

    public void Execute()
    {
        Action?.Invoke();
    }

    public void Decrement()
    {
        Delay--;
    }
}