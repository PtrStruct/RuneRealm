using System.Diagnostics;

namespace RuneRealm.Util;

public class TickRateManager
{
    private static Stopwatch _stopwatch;
    public TickRateManager()
    {
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public static void SleepIfRequired(double tickRate)
    {
        _stopwatch.Stop();
        var elapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;
        var sleepTime = CalculateSleepTime(elapsedMilliseconds, tickRate);

        if (sleepTime > TimeSpan.Zero)
        {
            Thread.Sleep(sleepTime);
        }
        else
        {
            WarnAboutDeficit(sleepTime, elapsedMilliseconds);
        }

        _stopwatch.Restart();
    }

    private static TimeSpan CalculateSleepTime(double elapsedMilliseconds, double tickRate)
    {
        return TimeSpan.FromMilliseconds(tickRate - elapsedMilliseconds);
    }

    private static void WarnAboutDeficit(TimeSpan sleepTime, double elapsedMilliseconds)
    {
        Console.WriteLine($"Server can't keep up! Elapsed: {elapsedMilliseconds} ms, Deficit: {-sleepTime.TotalMilliseconds} ms.");
    }
}