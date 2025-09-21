using UnityEngine;

[CreateAssetMenu(menuName = "Factory/Input/MRTKSpeech",fileName = "MRTKSpeechInput")]
public class MRTKSpeechnput : SpeechFactoryBase
{
    public override ISpeechInput CreateSpeechInput(SpeechEntryData data)
    {
        return new MRTKSpeech(data);
    }
}
