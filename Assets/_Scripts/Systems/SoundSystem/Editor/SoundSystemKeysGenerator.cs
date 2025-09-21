using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CustomSoundSystem.RunTime;
using UnityEngine;


namespace CustomSoundSystem.Editor
{
public class SoundSystemKeysGenerator : EditorWindow
{
    private static SoundSystemKeysGenerator window;
    private static string keyTypeName = "";
    private static string search = "";
    private static string lastSearchValue = "";
    private static bool doSearch => ! string.IsNullOrWhiteSpace(search) && search != "";
    private static List<string> newKeysPersistentData= new List<string>();
    private static List<string> tempList=new List<string>();
    private static List<EnumSearchList> searchList = new List<EnumSearchList>();
    private static List<bool> editList = new List<bool>();
    private static List<int> errorList = new List<int>();
    private static GUIStyle generateButton;
    private static GUIStyle titleStyle;
    private  GUIStyle errorStyle;
    private  GUIStyle labelStyle;
    private  GUIStyle textStyle;
    private static string pattern = "^[a-zA-Z_][a-zA-Z0-9_]*$";
    private Vector2 scrollPos;

    private bool start;
    
    public static void ShowWindow(string name)
    {
        keyTypeName = name;
        window= GetWindow<SoundSystemKeysGenerator>("Generate Enum");

        Init();
        InitTitleStyles();
        InitButtonsStyles();

        if (tempList.Count < 10)
        {
            window.minSize = new Vector2(300, EditorGUIUtility.singleLineHeight*(tempList.Count+8));
            window.maxSize = new Vector2(300, EditorGUIUtility.singleLineHeight*(tempList.Count+8));
        }
        else
        {
            window.minSize = new Vector2(300, EditorGUIUtility.singleLineHeight*17);
            window.maxSize = new Vector2(300, EditorGUIUtility.singleLineHeight*17);
        }
    }
    

    void OnGUI()
    {
        if (!start)
        {
            InitErrorStyles();
            labelStyle= new GUIStyle(GUI.skin.label);
            textStyle = new GUIStyle(GUI.skin.textField);
            start = true;
        }
    
        if (!doSearch)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Current Modified: {keyTypeName}",titleStyle);
        
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_ol_plus", "Add")))
            {
                tempList.Add("Value");
                editList.Add(true);
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField($"Current Modified Enum {keyTypeName}",titleStyle);
        }
     
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(EditorGUIUtility.IconContent("Search Icon"));
        EditorGUILayout.Space(-300);
        search= EditorGUILayout.TextField(search);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);


        if (doSearch)
        {
            CreateSearchList();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            for (var index = 0; index < searchList.Count; index++)
            {
                var inErrorList = errorList.Contains(searchList[index].originalListIndex);
                if (!Regex.IsMatch(searchList[index].value, pattern))
                {
                    if (!inErrorList)
                    {
                        errorList.Add(searchList[index].originalListIndex);
                    }
                }
                else if(inErrorList)
                {
                    errorList.Remove(searchList[index].originalListIndex);
                }
                if (tempList.Count(s => s.Equals(tempList[searchList[index].originalListIndex]))>1) errorList.Add(searchList[index].originalListIndex);
                else
                {
                    if (Regex.IsMatch(searchList[index].value, pattern))
                    {
                        errorList.Remove(searchList[index].originalListIndex);
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (editList[searchList[index].originalListIndex])
                {
                    searchList[index].value = EditorGUILayout.TextField( searchList[index].value,inErrorList? errorStyle: textStyle);
                    tempList[searchList[index].originalListIndex] = searchList[index].value;
                }
                else
                {
                    EditorGUILayout.LabelField(searchList[index].value,inErrorList? errorStyle: labelStyle);
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent(editList[searchList[index].originalListIndex]?"Update-Available":"d_editicon.sml", "Update")))
                {
                    editList[searchList[index].originalListIndex] = !editList[searchList[index].originalListIndex];
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Remove")))
                {
                    tempList.RemoveAt(searchList[index].originalListIndex);
                    errorList.Remove(index);
                    searchList.RemoveAt(index);

                }
                EditorGUILayout.EndHorizontal();
            
            
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            for (var index = 0; index < tempList.Count; index++)
            {
                var inErrorList = errorList.Contains(index);;
                if (!Regex.IsMatch(tempList[index], pattern))
                {
                    if (!inErrorList)
                    {
                        errorList.Add(index);
                    }
                }
                else if(inErrorList)
                {
                    errorList.Remove(index);
                }
                if (tempList.Count(s => s.Equals(tempList[index]))>1) errorList.Add(index);
                else
                {
                    if (Regex.IsMatch(tempList[index], pattern))
                    {
                        errorList.Remove(index);
                    }
                }
                
                EditorGUILayout.BeginHorizontal();
                if (editList[index])
                {
                    tempList[index] = EditorGUILayout.TextField(tempList[index],inErrorList? errorStyle: textStyle);

                }
                else
                {
                    EditorGUILayout.LabelField(tempList[index],inErrorList? errorStyle: labelStyle);
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent(editList[index]?"Update-Available":"d_editicon.sml", "Update")))
                {
                    editList[index] = !editList[index];
                }
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash", "Remove")))
                {
                    tempList.RemoveAt(index);
                    errorList.Remove(index);

                }
                
                EditorGUILayout.EndHorizontal();
            
            
            }
            EditorGUILayout.EndScrollView();
        }
     

        
        
        if (tempList.Count < 10)
        {
            window.minSize = new Vector2(300, EditorGUIUtility.singleLineHeight*(tempList.Count+8));
            window.maxSize = new Vector2(300, EditorGUIUtility.singleLineHeight*(tempList.Count+8));
        }
        

        GUILayout.FlexibleSpace();
        if (errorList.Count > 0)
        {
            EditorGUILayout.HelpBox("Not all of the enums you inserted have a valid enum name, or you have duplicates in the enum list",MessageType.Error);
            return;
        }
        if (GUILayout.Button("Generate Script",generateButton))
        {
            newKeysPersistentData.Clear();
            foreach (var e in tempList)
            {
                newKeysPersistentData.Add(e);
            }
            GenerateEnum();
            window.Close();
        }
        
        EditorGUILayout.Space(5);
    }
    
    
    private static void Init()
    {
        GetPersistentData();
        editList.Clear();
        tempList.Clear();
        editList.Clear();
        foreach (var ne in newKeysPersistentData)
        {
            tempList.Add(ne);
            editList.Add(false);
        }
    }
    
    private static void GetPersistentData()
    {
        newKeysPersistentData.Clear();
        Type enumType = string.Equals(keyTypeName, "SoundSourceKeys")
            ? SoundSystemReferences.SoundSourceKeysType
            : SoundSystemReferences.SoundTypeKeysType;
        if (enumType != null && enumType.IsEnum)
        {
            var enumValues = Enum.GetNames(enumType);
            foreach (var value in enumValues)
            {
                newKeysPersistentData.Add(value);
            }
        }
        
    }
    

    private void GenerateEnum()
    {
        newKeysPersistentData.Clear();
        foreach (var e in tempList)
        {
            newKeysPersistentData.Add(e);
        }
        var enumDefinition = new System.Text.StringBuilder();
        enumDefinition.AppendLine("namespace CustomSoundSystem");
        enumDefinition.AppendLine("{");
        enumDefinition.AppendLine($"public enum {keyTypeName}");
        enumDefinition.AppendLine("{");
        
        foreach (var value in newKeysPersistentData)
        {
            enumDefinition.AppendLine($"    {value},");
        }

        enumDefinition.AppendLine("}");
        enumDefinition.AppendLine("}");
        var folderPath = Path.Combine(SoundSystemReferences.InternalFolderPath,"Enums");
        string filePath = Path.Combine(folderPath, keyTypeName + ".cs");
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(filePath, enumDefinition.ToString());
        AssetDatabase.Refresh();
    }

    private static void InitButtonsStyles()
    {
        generateButton = new GUIStyle()
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.black, background =SoundSystemTextureUtility. MakeTex(2, 2, new Color(0.5f,1,0.7f)) },
            hover = { textColor = Color.gray, background = SoundSystemTextureUtility.MakeTex(2, 2, new Color(0.8f,1,0.7f))  },
            active = { textColor = new Color(0.8f,0.2f,0.5f), background = SoundSystemTextureUtility.MakeTex(2, 2, new Color(0.2f,0.4f,0.5f))}
        };
    }
    
    private static void InitTitleStyles()
    {
        titleStyle = new GUIStyle()
        {
            fontSize = 12,
            fontStyle = FontStyle.BoldAndItalic,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white, background = SoundSystemTextureUtility. MakeTex(2, 2, new Color(0.4f,0.2f,0.6f)) },
        };
    }
    
    private void InitErrorStyles()
    {
        errorStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white, background = SoundSystemTextureUtility.MakeTex(2, 2, new Color(1f,0,0)) },
        };
    }
    

    private void CreateSearchList()
    {
        if(lastSearchValue.Equals(search)) return;
        lastSearchValue = search; 
        searchList.Clear();
        for (int i = 0; i < tempList.Count(); i++)
        {
            var value = tempList[i].ToLower();
            if (value.Contains(search.ToLower()))
            {
                searchList.Add(new EnumSearchList(tempList[i],i));
            }
        }
    }
}

public class EnumSearchList
{
    public string value;
    public int originalListIndex;

    public EnumSearchList(string value, int originalListIndex)
    {
        this.value = value;
        this.originalListIndex = originalListIndex;
    }
}

}
