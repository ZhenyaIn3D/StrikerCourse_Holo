using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CustomSoundSystem.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CustomSoundSystem.Editor
{
[CustomEditor(typeof(SoundSystem))]
public class SoundSystemCustomEditor : UnityEditor.Editor
{
    private static SerializedObject so;
    private static ReorderableList list;
    private int selectedIndex;
    
    public static SoundSystem soundSystem;
    public static event Action<bool> OnVisibilityChanged;
    private void OnEnable()
    {
        so = serializedObject;
        soundSystem = (SoundSystem)serializedObject.targetObject;
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("soundUnits"), true, true, false,
            false);
        
        list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Sound Units"); };
        
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            
                
            
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, rect.height), element, GUIContent.none);
        };
        
        list.elementHeightCallback = (int index) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };
        
        
        
        list.onSelectCallback = (ReorderableList l) =>
        {
            selectedIndex= l.index;
        };
        

        
        OnVisibilityChanged?.Invoke(true);
        
    }
    

    private void OnDisable()
    {
        OnVisibilityChanged?.Invoke(false);
       
    }
    


    public override void OnInspectorGUI()
    {
        var width = GUILayout.Width(EditorGUIUtility.currentViewWidth/2-15);
        var height = GUILayout.Height(30);
        serializedObject.Update();
        EditorGUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(EditorGUIUtility.IconContent("SettingsIcon")))
        {
            var menu= new GenericMenu();
            menu.AddItem(new GUIContent("Build Version"),soundSystem.buildVersion,()=>soundSystem.buildVersion=!soundSystem.buildVersion);
            menu.AddItem(new GUIContent("Modify Sound Source Keys"),false,()=>SoundSystemKeysGenerator.ShowWindow("SoundSourceKeys"));
            
            menu.ShowAsContext();
        }
        SerializedProperty customClassProperty = serializedObject.FindProperty("soundMapper");
        GUILayout.Space(EditorGUIUtility.currentViewWidth/4);
        EditorGUILayout.LabelField("Sound Mapper");
        GUILayout.Space(-EditorGUIUtility.currentViewWidth/3.2f);
        EditorGUILayout.PropertyField(customClassProperty,GUIContent.none);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add/Update Sound Unit", width, height))
        {
            if (ReferenceEquals(SoundSystemReferences.SoundSourceKeysType,null)
                || ReferenceEquals(SoundSystemReferences.SoundTypeKeysType,null))
            {
                EditorUtility.DisplayDialog("Error","Please make sure you are already generated SoundSourceKeys Enum and SoundTypeKeys Enum",
                    "Close");
                return;
            }
            var soundSourceValues = Enum.GetNames(SoundSystemReferences.SoundSourceKeysType);
            var soundTypeValues = Enum.GetNames(SoundSystemReferences.SoundTypeKeysType);
            if (soundSourceValues.Length == 0 || soundTypeValues.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "Please Generate At least One SoundSource Enum And One SoundType Enum",
                    "Close");
                return;
            }
            SoundUnitListUtilityWindow.ShowWindow();
        }
        
        var arraySize = list.serializedProperty.arraySize;
        GUI.contentColor = arraySize == 0 ? Color.gray : Color.white;
        if (GUILayout.Button("Remove Sound Unit", width, height))
        {
            if(arraySize==0) return;
            if (selectedIndex >= list.serializedProperty.arraySize)
            {
                selectedIndex = list.serializedProperty.arraySize - 1;
            }
        
            list.serializedProperty.DeleteArrayElementAtIndex(selectedIndex);
        }
        GUI.contentColor = Color.white; GUILayout.EndHorizontal();
        EditorGUILayout.Space(10);
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    public static void UpdateSoundUnit(SoundUnit soundUnit)
    {
        so.ApplyModifiedProperties();
        soundSystem.Editor_UpdateSoundUnit(soundUnit);
    }

    public static void AddSoundUnit(SoundUnit soundUnit)
    {
        list.serializedProperty.arraySize++;
        so.ApplyModifiedProperties();
        soundSystem.Editor_SetNewAudioSource(soundUnit);
    }

    public static void ToggleLockScreen(bool shouldLock)
    {
        ActiveEditorTracker.sharedTracker.isLocked = shouldLock;
    }

    public static bool IsAudioSourceContained(GameObject go, string soundSource)
    { 
        return soundSystem.Editor_IsContainsAudioSource(go,soundSource);
    }
    
    public static GameObject GetAudioSourceGO(string soundSource)
    { 
        return soundSystem.Editor_GetAudioSourceGO(soundSource);
    }

    public static int GetListCount() => list.serializedProperty.arraySize;
    
    

}

public class SoundUnitListUtilityWindow : EditorWindow
{
    private static EditorWindow window;
    private static string[] soundKeys;
    private static GameObject selectedGameObject;
    private GameObject currentAssignGoToSoundUnit;
    private string soundSource;
    private string lastSoundSource;
    private bool pending;
    private bool playOnAwake;
    private bool useCustomSettings;
    private bool loop;
    private float volume = 100;
    private int priority = 128;
    private float spatialBlend = 0;
    private static bool ssVisible;
    private bool needToUpdate = true;
    private bool needToReset;
    private bool needToRestAfterAdding;
    private int currentSelection;

    private void OnEnable()
    {
        ssVisible = true;
        SoundSystemCustomEditor.ToggleLockScreen(true);
        SoundSystemCustomEditor.OnVisibilityChanged += IsSoundSystemInspectorIsVisible;
        needToRestAfterAdding = SoundSystemCustomEditor.GetListCount() == 0;
        soundKeys = Enum.GetNames(SoundSystemReferences.SoundSourceKeysType);
        soundSource = soundKeys[0];
        if (SoundSystemCustomEditor.soundSystem.Editor_Contains(soundSource))
        {
            GetValues();
            needToUpdate = false;
            lastSoundSource = soundSource; 
        }
        else
        {
            ResetToDefaultValues();
        }
        
        
    }

    private void OnDisable()
    {
        SoundSystemCustomEditor.ToggleLockScreen(false);
        SoundSystemCustomEditor.OnVisibilityChanged -= IsSoundSystemInspectorIsVisible;

    }

    public static void ShowWindow()
    {
        ActiveEditorTracker.sharedTracker.isLocked = true;
        selectedGameObject = null;
        window= GetWindow(typeof(SoundUnitListUtilityWindow), false,"Sound Unit Utility",false);
        window.minSize = new Vector2(400, 180);
        window.maxSize = new Vector2(400, 180);
        window.SaveChanges();
        window.Show();
    }
    



    private void OnGUI()
    {
        if (!ssVisible)
        {
            window.Close();
        }
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sound Source GO");
        GUILayout.Space(40);
        currentSelection = EditorGUI.Popup(new Rect(110,3,136,20), "",currentSelection,soundKeys);
        soundSource = soundKeys[currentSelection];
        if(lastSoundSource != soundSource)
        {
            ResetToDefaultValues();
        }
        else if (SoundSystemCustomEditor.soundSystem.Editor_Contains(soundSource))
        {
            if (needToUpdate)
            {
                GetValues();
                needToUpdate = false;
            }
        }

        if (lastSoundSource != soundSource)
        {
            needToUpdate = true;
            pending = false;
        }
        lastSoundSource = soundSource; 
        if(!pending)currentAssignGoToSoundUnit = SoundSystemCustomEditor.GetAudioSourceGO(soundSource);
        selectedGameObject = (GameObject)EditorGUILayout.ObjectField(selectedGameObject, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        playOnAwake = EditorGUILayout.Toggle("Play On Awake", playOnAwake);
        EditorGUI.LabelField(new Rect(150,0,100,100),"Priority");
        GUILayout.Space(15);
        priority = EditorGUILayout.IntSlider(priority, 0, 256);
        GUILayout.Space(15);
        EditorGUI.LabelField(new Rect(135,35,100,100),"Spatial Blend");
        spatialBlend = EditorGUILayout.Slider(spatialBlend, 0, 1);
        GUILayout.Space(10);
        useCustomSettings = EditorGUILayout.Toggle("Additional Settings", useCustomSettings);
      
    if (useCustomSettings)
    {
        loop = EditorGUILayout.Toggle("Loop", loop);
        GUILayout.Space(15);
        EditorGUI.LabelField(new Rect(150,120,100,100),"Volume");
        volume = EditorGUILayout.Slider(volume, 0, 1);
        window.minSize = new Vector2(400, 230);
        window.maxSize = new Vector2(400, 230);
    }
    else
    {
        window.minSize = new Vector2(400, 180);
        window.maxSize = new Vector2(400, 180);
    }
    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.Space(100);
    var width = GUILayout.Width(EditorGUIUtility.currentViewWidth/2-15);
    var height = GUILayout.Height(30);
    
    if (SoundSystemCustomEditor.IsAudioSourceContained(selectedGameObject,soundSource) &&!ReferenceEquals(selectedGameObject,currentAssignGoToSoundUnit))
    {
        var proceed= EditorUtility.DisplayDialog("",
            "The Game Object is already used for different Sound Source, Are you sure you want to assign it to another Sound Source",
            "Yes", "No");
        if (!proceed)
        {
            selectedGameObject =currentAssignGoToSoundUnit;
        }
        else
        {
            currentAssignGoToSoundUnit = selectedGameObject;
            pending = true;
        }
        
    }
    if (SoundSystemCustomEditor.soundSystem.Editor_Contains(soundSource))
    {
        if (GUILayout.Button("Update Sound Unit", width, height))
        {
            var audioSource= ReferenceEquals(selectedGameObject, null)?SoundSystemCustomEditor.soundSystem.Editor_GetAudioSource(soundSource):SetAudioSource();
            var soundUnit = new SoundUnit(soundSource,audioSource)
            {
                soundSourceSelection = currentSelection,
                useCustomSettings = useCustomSettings,
                volume = volume,
                loop = loop,
                playOnAwake = playOnAwake,
                priority = priority,
                spatialBlend =spatialBlend

            };
            SoundSystemCustomEditor.UpdateSoundUnit(soundUnit);
            pending = false;

        }

    }
    else 
    {
        if (!ReferenceEquals(selectedGameObject, null))
        {
            if (GUILayout.Button("Add Sound Unit", width, height))
            {
                
                var audioSource = SetAudioSource();
                
                var soundUnit = new SoundUnit(soundSource,audioSource)
                {
                    soundSourceSelection = currentSelection,
                    audioSource = audioSource,
                    useCustomSettings = useCustomSettings,
                    volume = volume,
                    loop = loop,
                    playOnAwake = playOnAwake,
                    priority = priority,
                    spatialBlend =spatialBlend

                };

                SoundSystemCustomEditor.AddSoundUnit(soundUnit);
                pending = false;
                if(needToRestAfterAdding) window.Close();
            }
        }
        else
        {
            GUILayout.Space(-100);
            EditorGUILayout.HelpBox("Audio Source GO Is Empty",MessageType.Error);
        }
    }
    
    GUILayout.EndHorizontal();
    
        
    }
    
    

    private AudioSource SetAudioSource()
    {
        if (!selectedGameObject.TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource=  selectedGameObject.AddComponent<AudioSource>();
        }
        return audioSource;
    }

    private static void IsSoundSystemInspectorIsVisible(bool isVisible)
    {
        ssVisible = isVisible;
    }

    private void ResetToDefaultValues()
    {
        selectedGameObject = null;
        playOnAwake=false;
        useCustomSettings=false;
        loop=false;
        volume = 100;
        priority = 128;
    }

    private void GetValues()
    {
        var soundUnit= SoundSystemCustomEditor.soundSystem.Editor_GetSoundUnit(this.soundSource);
        if (soundUnit.audioSource == null)
        {
            ResetToDefaultValues();
            return;
        }

        currentSelection = soundUnit.soundSourceSelection;
        selectedGameObject = soundUnit.audioSource.gameObject;
        playOnAwake = soundUnit.audioSource.playOnAwake;
        useCustomSettings = soundUnit.useCustomSettings;
        loop = soundUnit.audioSource.loop;
        volume = soundUnit.audioSource.volume;
        priority = soundUnit.audioSource.priority;
    }

}

}
