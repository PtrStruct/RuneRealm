using RuneRealm.Constants;
using RuneRealm.Util;

namespace RuneRealm.Network;

public class RSServer
{
    public bool IsRunning { get; private set; }

    public void Run()
    {
        IsRunning = true;
        Console.WriteLine("Server is running..");
        while (IsRunning)
        {
            Tick();
            TickRateManager.SleepIfRequired(ServerConfig.TICK_RATE);
        }
    }

    private void Tick()
    {
        
    }

    public void Stop()
    {
        IsRunning = false;
    }
}