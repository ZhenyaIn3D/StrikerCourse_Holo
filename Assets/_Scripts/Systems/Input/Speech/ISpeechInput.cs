using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeechInput : IDisposable
{
    public static ISpeechInput CreateDefault(SpeechEntryData data) => new MRTKSpeech(data);
    public SpeechEntryData data {get; set; }
    void StartRecognize();
    void StopRecognize();
    void AddEntry(SpeechEntry entry);
    void RemoveEntry(SpeechEntry entry);
    void SubscribeToAction(string keyword, Action action);
    void UnSubscribeToAction(string keyword, Action action);
}

public class SpeechEntry : IDisposable
{
    public readonly string keyword;
    private event Action action;
    private List<Action> subscribers= new List<Action>();
    private bool disposed;
 

    public SpeechEntry(string keyword)
    {
        this.keyword = keyword;
    }

    public override int GetHashCode()
    {
        return keyword.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        var other = (SpeechEntry)obj;
        return string.Equals(keyword, other.keyword);
    }
    
    public static bool operator ==(SpeechEntry left, SpeechEntry right)
    {
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(SpeechEntry left, SpeechEntry right)
    {
        return !(left == right);
    }

    public void Subscribe(Action actionToSubscribe)
    {
        action += actionToSubscribe;
        subscribers.Add(actionToSubscribe);
    }

    public void Unsubscribe(Action actionToRemove)
    {
        action -= actionToRemove;
        subscribers.Remove(this.action);
    }
    
    
    public void UnsubscribeAll()
    {
        for (int i = 0; i < subscribers.Count; i++)
        {
            action -= subscribers[i];
        }
        
        subscribers.Clear();
    }

    public void InvokeAction()
    {
        action?.Invoke();
    }

    ~SpeechEntry()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        
    }

    public virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                UnsubscribeAll();
            }
        }
    }
}

