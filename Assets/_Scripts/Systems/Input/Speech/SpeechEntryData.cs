using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Input/SpeechEntryData",fileName = "SpeechEntryData")]
public class SpeechEntryData : ScriptableObject
{
    public List<string> keywords;
}