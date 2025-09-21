using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<MainThreadDispatcher>();

            if (_instance) return _instance;

            _instance = new GameObject(nameof(MainThreadDispatcher)).AddComponent<MainThreadDispatcher>();

            return _instance;
        }
    }

    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void DoInMainThread(Action action)
    {
        _actions.Enqueue(action);
    }

    private void Update()
    {
        while (_actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}