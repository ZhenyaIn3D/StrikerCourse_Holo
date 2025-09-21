using System.Collections.Generic;
using System.Linq;

public static class TimerManager
{
    private static readonly List<Timer> timers = new List<Timer>();

    
    public static void RegisterTimer(Timer timer) => timers.Add(timer);
    public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

    public static void UpdateTimers()
    {
        foreach (var timer in timers.ToList())        
        {
            timer.Tick();
        }
    }

    public static void Clear() => timers.Clear();
}