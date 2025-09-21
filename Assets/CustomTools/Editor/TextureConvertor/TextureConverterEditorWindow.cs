using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TextureConverterEditorWindow : EditorWindow
{
    private static TextureConverterEditorWindow window;
    private static List<TextureData> data = new List<TextureData>();
    private Texture2D textureToConvert;
    

    [MenuItem("CustomTools/Utility/Texture Converter",priority = 1)]
    static void Init()
    {
        window = (TextureConverterEditorWindow)EditorWindow.GetWindow(typeof(TextureConverterEditorWindow));
        window.Show();
        var size = new Vector2(400, EditorGUIUtility.singleLineHeight*4);
        window.minSize = size;
        window.maxSize = size;
    }

    void OnGUI()
    {
        window.titleContent = new GUIContent("Texture Convertor");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Textures to Convert", EditorStyles.boldLabel);
        if (GUILayout.Button("Add Texture"))
        {
            TextureData newTextureData = new TextureData();
            newTextureData.texture = null;
            data.Add(newTextureData);
            var size = window.minSize;
            size.y =data.Count*EditorGUIUtility.singleLineHeight*4+EditorGUIUtility.singleLineHeight*3;
            window.minSize=size;
            window.maxSize = size;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < data.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            data[i].texture =
                EditorGUILayout.ObjectField("Texture " + (i + 1), data[i].texture, typeof(Texture2D), false) as
                    Texture2D;
            if (GUILayout.Button("Remove"))
            {
                data.RemoveAt(i);
                var size = window.minSize;
                size.y =data.Count*EditorGUIUtility.singleLineHeight*4+EditorGUIUtility.singleLineHeight*3;
                window.minSize=size;
                window.maxSize = size;
                EditorGUILayout.EndHorizontal();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUI.backgroundColor = data.Count > 0 ? Color.white : Color.gray;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Convert To Json"))
        {
            if (data.Count > 0)
            { 
               var path = EditorUtility.SaveFilePanel("Save JSON", "Assets","EncodedTextures", "json");
               EditorTextureUtility.ConvertTextureToJson(data,path);
            }
            else
            {
                Debug.LogError("Please add at least one texture to convert.");
            }
        }
        GUI.backgroundColor = Color.white;
        if (GUILayout.Button("Convert From Json"))
        {
            var path = EditorUtility.OpenFilePanel("Save JSON", "Assets",  "json");
            EditorTextureUtility.LoadTexturesFromJson(path);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
}


