using System;
using CustomSoundSystem.RunTime;
using UnityEditor;
using UnityEngine;

namespace CustomSoundSystem.Editor
{
    [CustomPropertyDrawer(typeof(SoundMap))]
    public class SoundMapCustomDrawer : PropertyDrawer
    {
        private bool missingEnumsErrorAlreadyShown;
        private static string[] soundTypeValues;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ReferenceEquals(SoundSystemReferences.SoundTypeKeysType,null)) return;
            soundTypeValues ??= Enum.GetNames(SoundSystemReferences.SoundTypeKeysType);

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var soundTypeRect = new Rect(position.x, position.y + 10, position.width / 2.4f, position.height / 2);

            var soundType = property.FindPropertyRelative("soundTypeKey");
            var selection = property.FindPropertyRelative("selection");
            selection.intValue = EditorGUI.Popup(soundTypeRect, selection.intValue, soundTypeValues);
            selection.intValue = Mathf.Clamp(selection.intValue, 0, soundTypeValues.Length);
            
            if (selection.intValue > soundTypeValues.Length - 1)
            {
                if(!missingEnumsErrorAlreadyShown) 
                {
                    EditorUtility.DisplayDialog("Error", "Your SoundType value could no be found. " +
                                                         "Please ensure you are not referencing value that is no longer exist",
                        "Close");
                    missingEnumsErrorAlreadyShown = true;
                }
                return;
            }
            soundType.stringValue = soundTypeValues[selection.intValue];


            SerializedProperty audioClips = property.FindPropertyRelative("audioClip");
            Rect foldoutRect = new Rect(position.x + position.width / 2, position.y + 10, position.width,
                EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Audio Clips");

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < audioClips.arraySize; i++)
                {
                    SerializedProperty elementProperty = audioClips.GetArrayElementAtIndex(i);
                    Rect elementRect = new Rect(position.x + position.width / 2,
                        position.y + 20 + EditorGUIUtility.singleLineHeight * (i + 1), position.width / 2,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(elementRect, elementProperty, GUIContent.none);
                }

                EditorGUI.indentLevel--;
            }

            Rect addButtonRect = new Rect(position.x + position.width - 20 * 2, position.y + 15, 20,
                EditorGUIUtility.singleLineHeight);
            Rect removeButtonRect = new Rect(position.x + position.width - 20, position.y + 15, 20,
                EditorGUIUtility.singleLineHeight);
            if (GUI.Button(addButtonRect, "+"))
            {
                AddElement(audioClips);
            }

            if (GUI.Button(removeButtonRect, "-") && audioClips.arraySize > 0)
            {
                RemoveElement(audioClips);
            }


            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }



        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var arraySize = property.FindPropertyRelative("audioClip").arraySize;
            float height = !property.isExpanded || arraySize == 0
                ? EditorGUIUtility.singleLineHeight * 2
                : (EditorGUIUtility.singleLineHeight) * (arraySize + 3);
            return height;
        }

        private void AddElement(SerializedProperty arrayProperty)
        {
            arrayProperty.arraySize++;
        }

        private void RemoveElement(SerializedProperty arrayProperty)
        {
            arrayProperty.arraySize--;
        }
    }
}
