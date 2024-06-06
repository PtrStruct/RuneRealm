using System.Diagnostics;

namespace RuneRealm.Util;

public class RSBenchmark
{
    public static void Eval(Action action, string benchmarkText)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        action.Invoke();

        stopwatch.Stop();
        Console.WriteLine($"{benchmarkText}: {stopwatch.ElapsedMilliseconds}ms");
    }
}