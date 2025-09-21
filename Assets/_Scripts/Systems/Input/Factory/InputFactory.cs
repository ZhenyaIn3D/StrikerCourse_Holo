using UnityEngine;

[CreateAssetMenu(menuName = "Factory/InputFactory", fileName = "Input Factory")]
public class InputFactory : ScriptableObject
{
    [SerializeField] private SpeechFactoryBase speechFactory;
    
    public ISpeechInput CreateSpeech(SpeechEntryData data) => speechFactory != null
        ? speechFactory.CreateSpeechInput(data)
        : ISpeechInput.CreateDefault(data);
    

}

