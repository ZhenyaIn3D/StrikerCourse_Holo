using System;
using System.Collections.Generic;
using System.IO;
using CustomSoundSystem.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CustomSoundSystem.Editor
{
    [CustomEditor(typeof(SoundTypeMapper))]
    public class SoundTypeMapperCustomEditor : UnityEditor.Editor
    {
        private SoundTypeMapper soundTypeMapper;
        private ReorderableList list;
        private List<int> duplicateList = new List<int>();
        private string soundPath = "";
        private SerializedProperty folderPath;

        private void OnEnable()
        {
            soundTypeMapper = (SoundTypeMapper)serializedObject.targetObject;
            folderPath = serializedObject.FindProperty("folderPath");
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("soundMap"), true, true, true,
                true);

            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Sound Units"); };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                if (duplicateList.Contains(index))
                {
                    EditorGUI.DrawRect(rect, new Color(0.7f, 0.2f, 0.2f, 1));

                }


                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, rect.height), element, GUIContent.none);
            };

            list.onAddCallback = (ReorderableList l) =>
            {
                if (ReferenceEquals(SoundSystemReferences.SoundTypeKeysType, null))
                {
                    EditorUtility.DisplayDialog("Error",
                        "Please make sure you are already generated the SoundTypeKeys Enum Type",
                        "Close");
                    return;
                }

                var soundTypeValues = Enum.GetNames(SoundSystemReferences.SoundTypeKeysType);
                if (soundTypeValues.Length == 0)
                {
                    EditorUtility.DisplayDialog("Error", "Please make sure you are already generated at least one SoundTypeKeys Enum",
                        "Close");

                    return;
                }

                list.serializedProperty.arraySize++;
            };


            list.elementHeightCallback = (int index) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element);
            };
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUI.Button(new Rect(20, 4, 20, 20), new GUIContent(EditorGUIUtility.FindTexture("SettingsIcon"))))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Change Sound Folder Path"), false, GetFolderPath);
                menu.AddItem(new GUIContent("Modify Sound Type Keys"), false, ModifySoundTypeEnum);
                menu.ShowAsContext();
            }

            if (GUI.Button(new Rect(45, 4, EditorGUIUtility.currentViewWidth - 50, 20), "Import New Sound"))
            {
                if (folderPath.stringValue.Equals(""))
                    folderPath.stringValue = EditorUtility.OpenFolderPanel("Select Sound Folder", "Assets", "");
                soundPath = EditorUtility.OpenFilePanel("Select Audio File", "Assets", "wav, mp3, ogg");
                if(String.Equals(soundPath,"")) return;
                // Debug.Log("!!!" + soundPath);
                // Debug.Log("!!!" + folderPath.stringValue);
                var fileName = soundPath.Split('/')[^1];
                var metaPath = $"{soundPath}.meta";
                if (File.Exists(metaPath))
                {
                    File.Move(soundPath, $"{folderPath.stringValue}/{fileName}");
                    File.Move(metaPath, $"{folderPath.stringValue}/{fileName}.meta");
                }
                else
                {
                    File.Copy(soundPath, $"{folderPath.stringValue}/{fileName}");
                }

                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space(40);

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                soundTypeMapper.Editor_CheckForDuplicates(ref duplicateList);
            }

        }

        private void GetFolderPath()
        {
            folderPath.stringValue = EditorUtility.OpenFolderPanel("Select Sound Folder", "Assets", "");
        }

        private void ModifySoundTypeEnum()
        {
            SoundSystemKeysGenerator.ShowWindow("SoundTypeKeys");
        }
    }

}
