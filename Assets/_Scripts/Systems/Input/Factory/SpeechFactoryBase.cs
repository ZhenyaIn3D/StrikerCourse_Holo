using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpeechFactoryBase : ScriptableObject
{
    public abstract ISpeechInput CreateSpeechInput(SpeechEntryData data);
}
