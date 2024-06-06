using RuneRealm.Constants;
using RuneRealm.Environment;
using RuneRealm.Util;

namespace RuneRealm.Network;

public class RSServer
{
    public bool IsRunning { get; private set; }

    public void Run()
    {
        IsRunning = true;
        ConnectionManager.Initialize();
        Console.WriteLine("Server is running..");
        while (IsRunning)
        {
            Tick();
            TickRateManager.SleepIfRequired(ServerConfig.TICK_RATE);
        }
    }

    private void Tick()
    {
        ConnectionManager.AcceptClients();
        World.Process();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}