using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

internal static class TimerBootstrap
{
    private static PlayerLoopSystem timerSystem;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    internal static void Initialize()
    {
        PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
        if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0))
        {
            Debug.LogError("Could not insert the Time Manager to the update loop");
            return;
        }

        PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeState;
        EditorApplication.playModeStateChanged += OnPlayModeState;

#endif

    }
    
#if UNITY_EDITOR
    static void OnPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            RemoveTimerManager<Update>(ref currentPlayerLoop);
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            TimerManager.Clear();
        }
    }
    
#endif
    
    static void RemoveTimerManager<T>(ref PlayerLoopSystem loop)
    {
        PlayerLoopUtils.RemoveSystem<T>(ref loop, in timerSystem);

    }


    static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index)
    {
        timerSystem = new PlayerLoopSystem()
        {
            type = typeof(TimerManager),
            updateDelegate = TimerManager.UpdateTimers,
            subSystemList = null
        };

        return PlayerLoopUtils.InsertSystem<T>(ref loop, in timerSystem, index);
    }



}