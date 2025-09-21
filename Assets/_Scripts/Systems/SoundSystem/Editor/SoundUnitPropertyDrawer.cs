using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CustomSoundSystem.RunTime;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomSoundSystem.Editor
{
    [CustomPropertyDrawer(typeof(SoundUnit))]

    public class SoundUnitPropertyDrawer : PropertyDrawer
    {
        private SoundSystem soundSystem;

        //private static TextureHolder textureHolder;
        private Texture2D playTex = Resources.Load<Texture2D>("Textures/play");
        private Texture2D pauseTex = Resources.Load<Texture2D>("Textures/pause");
        private Texture2D stopTex = Resources.Load<Texture2D>("Textures/stop");
        private Texture2D unpauseTex = Resources.Load<Texture2D>("Textures/unpause");
        private static List<string> soundSourceValues;
        private static string[] soundTypeValues;
        private bool missingEnumsErrorAlreadyShown;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ReferenceEquals(SoundSystemReferences.SoundSourceKeysType,null)
                || ReferenceEquals(SoundSystemReferences.SoundTypeKeysType,null))
                return;
            soundSourceValues ??= Enum.GetNames(SoundSystemReferences.SoundSourceKeysType).ToList();

            soundTypeValues ??= Enum.GetNames(SoundSystemReferences.SoundTypeKeysType);

            var needToConvertTextures = ReferenceEquals(playTex, null) || ReferenceEquals(stopTex, null) ||
                                        ReferenceEquals(pauseTex, null) || ReferenceEquals(unpauseTex, null);
            if (needToConvertTextures)
            {
                var textureHolder = new TextureHolder(SoundSystemReferences.textureData,
                    Path.Combine(SoundSystemReferences.InternalFolderPath,"Resources/Textures/"),
                    new string[4] { "play", "stop", "pause", "unpause" });

                playTex = Resources.Load<Texture2D>("Textures/play");
                pauseTex = Resources.Load<Texture2D>("Textures/pause");
                stopTex = Resources.Load<Texture2D>("Textures/stop");
                unpauseTex = Resources.Load<Texture2D>("Textures/unpause");
            }

            if (ReferenceEquals(soundSystem, null)) soundSystem = Object.FindObjectOfType<SoundSystem>();
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var playButtonRect = new Rect(position.x + position.width / 3.5f, position.y + 30, position.width / 20,
                position.height / 4);
            var pauseButtonRect = new Rect(playButtonRect.x + position.width / 20, position.y + 30, position.width / 20,
                position.height / 4);
            var stopButtonRect = new Rect(playButtonRect.x + position.width / 10, position.y + 30, position.width / 20,
                position.height / 4);
            var soundTypeRect = new Rect(position.x + 2 * position.width / 3f, position.y, position.width / 3.2f,
                position.height / 4);
            var soundSourceRect = new Rect(position.x, position.y, position.width / 3.2f, position.height / 4);
            var audioSourceRect = new Rect(position.x + position.width / 3f, position.y, position.width / 3.2f,
                position.height / 4);
            var useCustomSettingsRect = new Rect(position.x + position.width / 4.5f, position.y + 30,
                position.width - 90, position.height / 4);
            var loopRect = new Rect(position.x + position.width / 4.5f, position.y + 50, position.width - 90,
                position.height / 4);
            var volumeRect = new Rect(3 * position.x / 4 + position.width / 2, position.y + 50, position.width / 2,
                position.height / 4);
            var progressRect = new Rect(3 * position.x / 4 + position.width / 2, position.y + 30, position.width / 2,
                position.height / 4);
            var useCustomSettingsLabelRect = useCustomSettingsRect;
            useCustomSettingsLabelRect.x = position.x;

            GUI.enabled = false;
            var audioSource = property.FindPropertyRelative("audioSource");
            EditorGUI.PropertyField(audioSourceRect, audioSource, GUIContent.none);
            GUI.enabled = true;

            var soundType = property.FindPropertyRelative("soundTypeKey");
            var soundTypeSelection = property.FindPropertyRelative("soundTypeSelection");
            soundTypeSelection.intValue = EditorGUI.Popup(soundTypeRect, soundTypeSelection.intValue, soundTypeValues);
            soundType.stringValue = soundTypeValues[soundTypeSelection.intValue];

            var soundSourceSelection = property.FindPropertyRelative("soundSourceSelection");
            var soundSource = property.FindPropertyRelative("soundSourceKeys");
            soundSourceSelection.intValue = Mathf.Clamp(soundSourceSelection.intValue, 0, soundSourceValues.Count);
            if (soundSourceSelection.intValue > soundSourceValues.Count - 1)
            {
                if(!missingEnumsErrorAlreadyShown) 
                {
                    EditorUtility.DisplayDialog("Error", "Your SoundSource value could no be found. " +
                                                         "Please ensure you are not referencing value that is no longer exist",
                    "Close");
                    missingEnumsErrorAlreadyShown = true;
                }
                return;
            }
            soundSource.stringValue = soundSourceValues[soundSourceSelection.intValue];
            EditorGUI.LabelField(soundSourceRect, soundSource.stringValue);

            EditorGUI.LabelField(useCustomSettingsLabelRect, "Custom Settings");
            var useCustomSettings = property.FindPropertyRelative("useCustomSettings");

            EditorGUI.PropertyField(useCustomSettingsRect, useCustomSettings, GUIContent.none);
            if (useCustomSettings.boolValue)
            {
                var loopLabelRect = loopRect;
                loopLabelRect.x = position.x;
                var volumeLabelRect = volumeRect;
                volumeLabelRect.x = position.x + position.width / 3.5f;
                EditorGUI.LabelField(loopLabelRect, "Loop");
                var loop = property.FindPropertyRelative("loop");
                loop.boolValue = EditorGUI.Toggle(loopRect, GUIContent.none, loop.boolValue);
                EditorGUI.LabelField(volumeLabelRect, "Volume");
                var volume = property.FindPropertyRelative("volume");
                volume.floatValue = EditorGUI.Slider(volumeRect, GUIContent.none, volume.floatValue, 0, 1);
                var su = soundSystem.Editor_GetSoundUnit(soundSourceSelection.intValue);
                if (su.loop != loop.boolValue || Math.Abs(su.volume - volume.floatValue) > 0.01f)
                {

                    su.loop = loop.boolValue;
                    su.volume = volume.floatValue;
                    soundSystem.Editor_UpdateSoundUnit(su);

                }
            }
            else
            {
                var su = soundSystem.Editor_GetSoundUnit(soundSourceSelection.intValue);
                if (su.loop || Math.Abs(su.volume - 1) > 0.01f)
                {
                    su.loop = false;
                    su.volume = 1;
                    soundSystem.Editor_UpdateSoundUnit(su);

                }
            }

            var progressBar = property.FindPropertyRelative("progress").floatValue;

            var isPaused = property.FindPropertyRelative("isPaused").boolValue;

            EditorGUI.ProgressBar(progressRect, progressBar, "Progress");
            if (GUI.Button(playButtonRect, new GUIContent(playTex)))
            {
                soundSystem.Editor_Play(soundSource.stringValue, soundType.stringValue);
            }

            if (GUI.Button(pauseButtonRect, new GUIContent(isPaused ? unpauseTex : pauseTex)))
            {
                if (progressBar == 0) return;
                if (isPaused)
                {
                    soundSystem.Editor_UnPause(soundSourceSelection.intValue);
                }
                else
                {
                    soundSystem.Editor_Pause(soundSourceSelection.intValue);
                }
            }

            if (GUI.Button(stopButtonRect, new GUIContent(stopTex)))
            {
                var soundSourceValue = soundSourceValues.IndexOf(soundSource.stringValue);
                soundSystem.Editor_Stop(soundSourceValue);
            }



            EditorGUI.indentLevel = indent;



            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4;
        }


    }

    public class TextureHolder
    {
        public Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public TextureHolder(string jsonPath, string[] textureName)
        {
            SoundSystemTextureUtility.LoadTexturesFromJson(jsonPath);
            for (int i = 0; i < textureName.Length; i++)
            {
                textures.Add(textureName[i], Resources.Load<Texture2D>(textureName[i]));
            }
        }

        public TextureHolder(string[] encode, string jsonPath, string[] textureNames)
        {
            SoundSystemTextureUtility.LoadTexturesFromJson(encode, jsonPath, textureNames);
            for (int i = 0; i < textureNames.Length; i++)
            {
                textures.Add(textureNames[i], Resources.Load<Texture2D>(textureNames[i]));
            }
        }
    }

}
