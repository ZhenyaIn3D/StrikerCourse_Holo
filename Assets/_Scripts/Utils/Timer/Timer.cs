using System;
using UnityEngine;

public abstract class Timer : IDisposable
{
    private bool disposed;
    protected float initialTime;
    protected float Time { get; set; }
    public bool IsRunning { get; protected set; }
    
    public float Progress => Time / initialTime;
    
    public Action OnTimerStart = delegate { };
    public Action OnTimerStop = delegate { };

    protected Timer(float value) {
        initialTime = value;
        IsRunning = false;
    }

    public void Start() {
        Time = initialTime;
        if (!IsRunning) {
            IsRunning = true;
            OnTimerStart.Invoke();
            TimerManager.RegisterTimer(this);
        }
    }

    public void Stop() {
        if (IsRunning) {
            IsRunning = false;
            OnTimerStop.Invoke();
            TimerManager.DeregisterTimer(this);
        }
    }
    
    public void Resume() => IsRunning = true;
    public void Pause() => IsRunning = false;
    
    public abstract void Tick();

    ~Timer()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    public virtual void Dispose(bool diposing)
    {
        if(disposed) return;
        if (diposing)
        {
            TimerManager.DeregisterTimer(this);
        }

        disposed = true;
    }
    
}

public class CountdownTimer : Timer 
{
    public CountdownTimer(float value) : base(value) { }

    public override void Tick() {
        if (IsRunning && Time > 0)
        {
            Time -= UnityEngine.Time.deltaTime;
        }
        
        if (IsRunning && Time <= 0) {
            Stop();
        }
    }
    
    public bool IsFinished => Time <= 0;
    
    public void Reset() => Time = initialTime;
    
    public void Reset(float newTime) {
        initialTime = newTime;
        Reset();
    }

    public float GetNormalizeTime()
    {
        return 1 - Progress;
    }
}

public class StopwatchTimer : Timer {
    public StopwatchTimer() : base(0) { }

    public override void Tick() {
        if (IsRunning)
        {
            Time += UnityEngine.Time.deltaTime;
        }
    }
    
    public void Reset() => Time = 0;
    
    public float GetTime() => Time;
}



