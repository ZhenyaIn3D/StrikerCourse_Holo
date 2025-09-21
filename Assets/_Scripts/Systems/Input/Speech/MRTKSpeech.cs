using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;

public class MRTKSpeech : ISpeechInput
{
    public KeywordRecognizer keywordRecognizer;
    private Dictionary<string, SpeechEntry> entries= new Dictionary<string, SpeechEntry>();
    
    public SpeechEntryData data { get; set; }

    public MRTKSpeech(SpeechEntryData data)
    {
        this.data = data;
        foreach (var keyword in data.keywords)
        {
           AddEntry(new SpeechEntry(keyword)); 
        }
        
        keywordRecognizer = new KeywordRecognizer(data.keywords.ToArray());
    }
    
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (entries.TryGetValue(args.text, out var entry))
        {
            entry.InvokeAction();
        }
    }
    

    public void StartRecognize()
    {
        SubscribeToListen();
        keywordRecognizer.Start();
    }

    public void StopRecognize()
    {
        keywordRecognizer.Stop();
        UnSubscribeToListen();
    }

    public void AddEntry(SpeechEntry entry)
    {
        entries.Add(entry.keyword,entry);
    }

    public void RemoveEntry(SpeechEntry entry)
    {
        entries.Remove(entry.keyword);
    }

    public void SubscribeToAction(string keyword, Action action)
    {
        if (entries.TryGetValue(keyword, out var entry))
        {
            entry.Subscribe(action);
        }
    }

    public void UnSubscribeToAction(string keyword, Action action)
    {
        if (entries.TryGetValue(keyword, out var entry))
        {
            entry.Unsubscribe(action);
        }
    }


    private void SubscribeToListen()
    {
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
    }
    
    private void UnSubscribeToListen()
    {
        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
    }

    public void Dispose()
    {
        foreach (var entry in entries.Values)
        {
            entry.Dispose();
        }
    }
}
