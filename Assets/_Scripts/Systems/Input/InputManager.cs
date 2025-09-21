using System;
using UnityEngine;


public class InputManager :MonoBehaviour
{
   [SerializeField] private InputFactory inputFactory;
   
   private ISpeechInput speech;

   private void Awake()
   {
       Debug.Log(inputFactory == null);
       speech = inputFactory.CreateSpeech(Resources.Load<SpeechEntryData>("Input/Speech/MRTKSpeechKeywords"));
       Debug.Log(speech == null);
       speech.StartRecognize();
   }
   
   private void OnDestroy()
   {
      speech.Dispose();
   }
   
   public void StartListening() => speech.StartRecognize();
   public void StopListening() => speech.StopRecognize();

   public void SubscribeToSpeech(string keyword, Action action)
   {
       speech.SubscribeToAction(keyword, action);
   }
   public void UnSubscribeToSpeech(string keyword, Action action)
   {
       speech.UnSubscribeToAction(keyword, action);
   }
}
