using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomSoundSystem.RunTime
{
    [CreateAssetMenu(menuName = "Data/Sound Mapper", fileName = "New Sound Mapper")]
    public class SoundTypeMapper : ScriptableObject
    {
        private static List<string> soundTypeValues;

        public string folderPath = "";
        public SoundMap[] soundMap;

        private void OnEnable()
        {
            if(ReferenceEquals(SoundSystemReferences.SoundTypeKeysType,null)) return;
            soundTypeValues ??= Enum.GetNames(SoundSystemReferences.SoundTypeKeysType).ToList();
        }


        public void Editor_CheckForDuplicates(ref List<int> duplicateList)
        {
            duplicateList.Clear();
            var hash = new HashSet<SoundMap>();
            for (var i = 0; i < soundMap.Length; i++)
            {
                var sm = soundMap[i];
                if (hash.Add(sm) == false)
                {
                    duplicateList.Add(i);
                }
            }
        }

        [CanBeNull]
        public AudioClip GetAudio(string soundType)
        {
            var ac = soundMap.FirstOrDefault(sm => sm.soundTypeKey == soundType).audioClip;
            if (ReferenceEquals(ac, null))
            {
                Debug.LogError("There is not a matching audio clip");
                return null;
            }

            if (ac.Length == 0) return null;
            var index = ac.Length > 1 ? Random.Range(0, ac.Length) : 0;
            return ac[index];
        }

        [CanBeNull]
        public AudioClip Editor_GetAudio(int soundTypeIndex)
        {
            var soundType = soundTypeValues[soundTypeIndex];
            var ac = soundMap.FirstOrDefault(sm => sm.soundTypeKey == soundType).audioClip;
            if (ReferenceEquals(ac, null))
            {
                Debug.LogError("There is not a matching audio clip");
                return null;
            }

            if (ac.Length == 0) return null;
            var index = ac.Length > 1 ? Random.Range(0, ac.Length) : 0;
            return ac[index];
        }
    }

    [System.Serializable]
    public struct SoundMap
    {
        public string soundTypeKey;
        public AudioClip[] audioClip;
        public int selection;

        public static bool operator ==(SoundMap r, SoundMap l)
        {
            return r.soundTypeKey == l.soundTypeKey;
        }

        public static bool operator !=(SoundMap r, SoundMap l)
        {
            return r.soundTypeKey != l.soundTypeKey;
        }

        public override bool Equals(object obj)
        {
            if (obj is SoundMap)
            {
                SoundMap other = (SoundMap)obj;
                return this == other;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return soundTypeKey.GetHashCode();
        }
    }

}

