using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSoundSystem.RunTime
{

   public partial class SoundSystem : MonoBehaviour
   {
      [SerializeField] private SoundTypeMapper soundMapper;
      [SerializeField] private List<SoundUnit> soundUnits= new List<SoundUnit>();
      private Dictionary<string, SoundUnit> runTimeSoundUnits = new Dictionary<string, SoundUnit>();
      private SoundUnit currentSoundUnit;
      private readonly List<SoundUnit> currentlyPlayingSoundUnits = new List<SoundUnit>();
      private readonly bool defaultLoopSetting = false;
      private readonly float defaultVolume = 100;
      public bool buildVersion;



      void Awake()
      {
         InitializeRunTimeData();
      }
      private void InitializeRunTimeData()
      {
         for (var i = 0; i < soundUnits.Count; i++)
         {
            var su = soundUnits[i];
            if (runTimeSoundUnits.ContainsValue(su))
            {
               Debug.LogWarning(
                  $"The Sound Type {su.soundTypeKey} Appear More Than Once In Index {i}. Please Remove The Duplicate");
               return;
            }
            
            runTimeSoundUnits.Add(su.soundSourceKeys, su);
         }

         if(buildVersion) soundUnits = null;

      }

      public void Play(string soundSource, string soundType)
      {
         if(!ContainSoundSourceKey(soundSource)) return;
         currentSoundUnit = runTimeSoundUnits[soundSource];
         UpdateSoundUnit(currentSoundUnit);
         currentSoundUnit.audioSource.clip = soundMapper.GetAudio(soundType);
         currentSoundUnit.audioSource.time = 0;
         currentSoundUnit.audioSource.Play();
         currentSoundUnit.isPaused = false;

         currentlyPlayingSoundUnits.Add(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014

      }


      public void Stop(string soundSource)
      {
         if(!ContainSoundSourceKey(soundSource)) return;
         currentSoundUnit = runTimeSoundUnits[soundSource];
         currentSoundUnit.audioSource.Stop();
         currentSoundUnit.isPaused = false;
         currentSoundUnit.progress = 0;

         currentlyPlayingSoundUnits.Remove(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }


      public void Pause(string soundSource)
      {
         if(!ContainSoundSourceKey(soundSource)) return;
         currentSoundUnit = runTimeSoundUnits[soundSource];
         currentSoundUnit.audioSource.Pause();
         currentSoundUnit.isPaused = true;

         currentlyPlayingSoundUnits.Remove(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }

      public void UnPause(string soundSource)
      {
         if(!ContainSoundSourceKey(soundSource)) return;
         currentSoundUnit = runTimeSoundUnits[soundSource];
         if (!currentSoundUnit.isPaused) return;
         UpdateSoundUnit(currentSoundUnit);
         currentSoundUnit.audioSource.time = currentSoundUnit.audioSource.clip.length * currentSoundUnit.progress;
         currentSoundUnit.audioSource.Play();
         currentSoundUnit.isPaused = false;


         currentlyPlayingSoundUnits.Add(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }

      public void UpdateSoundUnit(SoundUnit soundUnit)
      {
         if(!ContainSoundSourceKey(soundUnit.soundSourceKeys)) return;
         currentSoundUnit = runTimeSoundUnits[soundUnit.soundSourceKeys];
         currentSoundUnit.soundSourceSelection = soundUnit.soundSourceSelection;
         currentSoundUnit.soundSourceKeys = soundUnit.soundSourceKeys;
         currentSoundUnit.audioSource = soundUnit.audioSource;
         currentSoundUnit.useCustomSettings = soundUnit.useCustomSettings;
         currentSoundUnit.loop = soundUnit.loop;
         currentSoundUnit.volume = soundUnit.volume;
         currentSoundUnit.playOnAwake = soundUnit.playOnAwake;
         currentSoundUnit.priority = soundUnit.priority;
         currentSoundUnit.spatialBlend = soundUnit.spatialBlend;

         if (currentSoundUnit.useCustomSettings)
         {
            currentSoundUnit.audioSource.loop = soundUnit.loop;
            currentSoundUnit.audioSource.volume = soundUnit.volume;
         }

         currentSoundUnit.audioSource.playOnAwake = soundUnit.playOnAwake;
         currentSoundUnit.audioSource.priority = soundUnit.priority;
         currentSoundUnit.audioSource.spatialBlend = soundUnit.spatialBlend;
      }

      public void ResetSettings(string soundSource)
      {
         if(!ContainSoundSourceKey(soundSource)) return;
         currentSoundUnit = runTimeSoundUnits[soundSource];
         currentSoundUnit.useCustomSettings = false;
         currentSoundUnit.audioSource.loop = defaultLoopSetting;
         currentSoundUnit.audioSource.volume = defaultVolume;

      }

      private async Task GetProgression()
      {
         while (currentlyPlayingSoundUnits.Count > 0)
         {
            foreach (var su in currentlyPlayingSoundUnits)
            {
               await Task.Yield();
               var playbackTime = su.audioSource.time;
               var duration = su.audioSource.clip.length;
               su.progress = (playbackTime / duration);
            }
         }
      }

      public float GetSoundUnitProgression(string soundSourceKey)
      {
         return currentSoundUnit.progress;
      }

      public float GetCurrentClipLength(string soundSourceKey)
      {
         return currentSoundUnit.audioSource.clip.length;
      }

      private bool ContainSoundSourceKey(string soundSourceKey)
      {
         if (!runTimeSoundUnits.ContainsKey(soundSourceKey))
         {
            Debug.LogError("Could not found the Sound Source Key in the dictionary." +
                           "Please make sure you assigned in the editor or used SoundSystem.SetNewSoundUnit() method to assign during run time");
            return false;
         }

         return true;
      }

      public AudioSource GetAudioSource(string soundSource)
      {
         return runTimeSoundUnits[soundSource].audioSource;
      }


      public SoundUnit GetSoundUnit(string soundSource)
      {
         return runTimeSoundUnits[soundSource];
      }
      
      
      public void SetAudioSourceToSoundUnit(string soundSource,AudioSource audioSource)
      {
         runTimeSoundUnits[soundSource].audioSource = audioSource;
         if (soundUnits != null)
            soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource)!.audioSource = audioSource;
      }
      
      public bool SetNewSoundUnit(string soundSource,AudioSource audioSource)
      {
         var su = new SoundUnit(soundSource,audioSource);
         try
         {
            runTimeSoundUnits.Add(soundSource,su);
            if(!ReferenceEquals(soundUnits,null)) soundUnits.Add(su);
            return true;
         }
         catch
         {
            Debug.LogError("Could not add the sound unit to the list check you did not try to add already existed sound source key");
            return false;
         }
         
      }
      
      public bool SetNewSoundUnit(string soundSource,SoundUnit soundUnit)
      {
         try
         {
            runTimeSoundUnits.Add(soundSource,soundUnit);
            if(!ReferenceEquals(soundUnits,null)) soundUnits.Add(soundUnit);
            return true;
         }
         catch
         {
            Debug.LogError("Could not add the sound unit to the list check you did not try to add already existed sound source key");
            return false;
         }
         
      }

      public void DeleteSoundUnit(string soundSource)
      {
         runTimeSoundUnits.Remove(soundSource);
         var su = soundUnits.FirstOrDefault(s => s.soundSourceKeys == soundSource);
         if(!ReferenceEquals(soundUnits,null)) soundUnits.Remove(su);
      }

   }
   

   /// <summary>
   /// Holds All The Editor Related Code
   /// </summary>
   public partial class SoundSystem
   {
      public void Editor_CheckForDuplicates(ref List<int> duplicateList)
      {
         duplicateList.Clear();
         var hash = new HashSet<SoundUnit>();
         for (var i = 0; i < soundUnits.Count; i++)
         {
            var su = soundUnits[i];
            if (hash.Add(su) == false)
            {
               duplicateList.Add(i);
            }
         }
      }


      public void Editor_Play(string soundSource, string soundType)
      {
         currentSoundUnit = soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource);
         if (ReferenceEquals(currentSoundUnit, null)) return;
         Editor_UpdateSoundUnit(currentSoundUnit);
         currentSoundUnit.audioSource.clip = soundMapper.GetAudio(soundType);
         currentSoundUnit.audioSource.time = 0;
         currentSoundUnit.isPaused = false;
         currentSoundUnit.audioSource.Play();

         currentlyPlayingSoundUnits.Add(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014

      }

      public void Editor_Stop(int index)
      {
         currentSoundUnit = soundUnits.FirstOrDefault(su => su.soundSourceSelection == index);
         if (ReferenceEquals(currentSoundUnit, null)) return;
         currentSoundUnit.isPaused = false;
         currentSoundUnit.audioSource.Stop();
         currentSoundUnit.progress = 0;

         currentlyPlayingSoundUnits.Remove(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }

      public void Editor_Pause(int index)
      {
         currentSoundUnit = soundUnits.FirstOrDefault(su => su.soundSourceSelection == index);
         if (ReferenceEquals(currentSoundUnit, null)) return;
         currentSoundUnit.audioSource.Pause();
         currentSoundUnit.isPaused = true;

         currentlyPlayingSoundUnits.Remove(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }

      public void Editor_UnPause(int index)
      {
         currentSoundUnit = soundUnits.FirstOrDefault(su => su.soundSourceSelection == index);
         if (ReferenceEquals(currentSoundUnit, null)) return;
         if (!currentSoundUnit.isPaused) return;
         Editor_UpdateSoundUnit(currentSoundUnit);
         currentSoundUnit.audioSource.time = currentSoundUnit.audioSource.clip.length * currentSoundUnit.progress;
         currentSoundUnit.audioSource.Play();
         currentSoundUnit.isPaused = false;

         currentlyPlayingSoundUnits.Add(currentSoundUnit);
#pragma warning disable CS4014
         GetProgression();
#pragma warning restore CS4014
      }
      

      public void Editor_SetNewAudioSource(SoundUnit soundUnit)
      {
         soundUnits[^1].soundSourceKeys = soundUnit.soundSourceKeys;
         soundUnits[^1].audioSource = soundUnit.audioSource;
         soundUnits[^1].useCustomSettings = soundUnit.useCustomSettings;
         soundUnits[^1].loop = soundUnit.loop;
         soundUnits[^1].volume = soundUnit.volume;
         soundUnits[^1].playOnAwake = soundUnit.playOnAwake;
         soundUnits[^1].priority = soundUnit.priority;
         soundUnits[^1].spatialBlend = soundUnit.spatialBlend;
         soundUnits[^1].soundSourceSelection = soundUnit.soundSourceSelection;

         if (soundUnits[^1].useCustomSettings)
         {
            soundUnits[^1].audioSource.loop = soundUnit.loop;
            soundUnits[^1].audioSource.volume = soundUnit.volume;
         }

         soundUnits[^1].audioSource.loop = soundUnit.loop;
         soundUnits[^1].audioSource.volume = soundUnit.volume;
         soundUnits[^1].audioSource.playOnAwake = soundUnit.playOnAwake;
         soundUnits[^1].audioSource.priority = soundUnit.priority;
         soundUnits[^1].audioSource.spatialBlend = soundUnit.spatialBlend;
      }
      
      public void Editor_UpdateSoundUnit(SoundUnit soundUnit)
      {
         currentSoundUnit = soundUnits.FirstOrDefault(su => su.soundSourceSelection == soundUnit.soundSourceSelection);
         currentSoundUnit.soundSourceKeys = soundUnit.soundSourceKeys;
         currentSoundUnit.audioSource = soundUnit.audioSource;
         currentSoundUnit.useCustomSettings = soundUnit.useCustomSettings;
         currentSoundUnit.loop = soundUnit.loop;
         currentSoundUnit.volume = soundUnit.volume;
         currentSoundUnit.playOnAwake = soundUnit.playOnAwake;
         currentSoundUnit.priority = soundUnit.priority;
         currentSoundUnit.spatialBlend = soundUnit.spatialBlend;
         currentSoundUnit.soundSourceSelection = soundUnit.soundSourceSelection;

         if (currentSoundUnit.useCustomSettings)
         {
            currentSoundUnit.audioSource.loop = soundUnit.loop;
            currentSoundUnit.audioSource.volume = soundUnit.volume;
         }

         currentSoundUnit.audioSource.playOnAwake = soundUnit.playOnAwake;
         currentSoundUnit.audioSource.priority = soundUnit.priority;
         currentSoundUnit.audioSource.spatialBlend = soundUnit.spatialBlend;
      }


      public AudioSource Editor_GetAudioSource(string soundSource)
      {
         return soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource).audioSource;
      }


      public SoundUnit Editor_GetSoundUnit(string soundSource)
      {
         return soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource);
      }

      public SoundUnit Editor_GetSoundUnit(int index)
      {
         return soundUnits.FirstOrDefault(su => su.soundSourceSelection == index);
      }


      private void Editor_ResetProgressBar()
      {
         foreach (var su in soundUnits)
         {
            su.progress = 0;
         }
      }

      public bool Editor_Contains(string soundSource)
      {
         if (ReferenceEquals(soundUnits, null)) return false;
         var soundUnit = soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource);
         return !ReferenceEquals(soundUnit, null);
      }

      public bool Editor_IsContainsAudioSource(GameObject go, string soundSource)
      {
         if (ReferenceEquals(soundUnits, null)) return false;
         var soundUnit = soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource);
         return !ReferenceEquals(soundUnits.FirstOrDefault(su => ReferenceEquals(su.audioSource.gameObject, go)), null);
      }

      public GameObject Editor_GetAudioSourceGO(string soundSource)
      {
         if (ReferenceEquals(soundUnits, null)) return null;
         return soundUnits.FirstOrDefault(su => su.soundSourceKeys == soundSource)?.audioSource.gameObject;
      }
   }
   

   [Serializable]
   public class SoundUnit
   {
      public AudioSource audioSource;
      public AudioClip audioClip;
      public bool useCustomSettings;
      public bool loop;
      public bool isPaused;
      public float volume;
      public float progress;
      public bool playOnAwake;
      public int priority;
      public float spatialBlend;
      public string soundSourceKeys;
      public string soundTypeKey;
      public int soundSourceSelection;
      public int soundTypeSelection;

      public SoundUnit(string soundSourceKey,AudioSource audioSource)
      {
         this.soundSourceKeys = soundSourceKey;
         this.audioSource = audioSource;
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(obj, null) || this.GetType() != obj.GetType()) return false;
         SoundUnit other = (SoundUnit)obj;
         return soundSourceKeys == other.soundSourceKeys;
      }

      public override int GetHashCode()
      {
         return soundSourceKeys.GetHashCode();
      }


      public static bool operator ==(SoundUnit left, SoundUnit right)
      {
         if (ReferenceEquals(left, right))
         {
            return true;
         }

         if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
         {
            return false;
         }

         return left.soundSourceKeys == right.soundSourceKeys;
      }

      public static bool operator !=(SoundUnit left, SoundUnit right)
      {
         return left.soundSourceKeys != right.soundSourceKeys;
      }

   }

}

