using System;
using CustomTools.Utility;
using UnityEditor;
using UnityEngine;

namespace CustomTools.CustomImporter
{


    public class AssetPipelineWindows : EditorWindow
    {
        private static AssetPipelineWindows window;
        private bool showCustomShaderProperties;

        private bool canStartImport =>
            ImportManager.Instance.UseCustomAssetPipLine && !ImportManager.Instance.UseCustomSubFolder
            || ImportManager.Instance.UseCustomSubFolder && !ImportManager.Instance.SubFolder.Equals("");

        [MenuItem("CustomTools/AssetImporter &w")]
        public static void ShowWindow()
        {
            window = GetWindow<AssetPipelineWindows>("Asset Importer", true);
            window.minSize = new Vector2(400, 160);
            window.maxSize = new Vector2(400, 160);
            window.SaveChanges();
            window.Show();

            ImportManager.Instance.UseCustomAssetPipLine = true;
        }

        private void OnDisable()
        {
            ImportManager.Instance.UseCustomAssetPipLine = false;
        }

        private void OnGUI()
        {
            var importManager = ImportManager.Instance;
            // importManager.UseCustomAssetPipLine =
            //     GUILayout.Toggle(importManager.UseCustomAssetPipLine, "Use Custom Asset Pipeline");
            EditorGUILayout.Separator();
            importManager.UseCustomSubFolder =
                GUILayout.Toggle(importManager.UseCustomSubFolder, "Use Custom Sub Folder");

            if (importManager.UseCustomSubFolder)
            {
                importManager.SubFolder = EditorGUILayout.TextField("Custom Sub Folder", importManager.SubFolder);
            }
            else
            {
                var style = new GUIStyle();
                style.richText = true;
                var color = "grey";
                EditorGUILayout.LabelField($"<size=12><color={color}>Custom Sub Folder</color></size>", style);
            }

            importManager.UseCustomMaterial = GUILayout.Toggle(importManager.UseCustomMaterial, "Use Custom Material");

            if (importManager.UseCustomMaterial)
            {
                if (GUILayout.Button("Custom Shader"))
                {
                    importManager.ShaderPath = EditorUtility.OpenFilePanel("Shader Path", "Assets", "*shader");
                }

                if (!importManager.ShaderPath.Equals(""))
                {
                    var split = importManager.ShaderPath.Split('/');
                    var shaderName  = split[^1];
                    var style = new GUIStyle();
                    style.richText = true;
                    var color = "grey";
                    EditorGUILayout.LabelField($"<size=12><color={color}>Shader Name: {shaderName}</color></size>", style);
                }

                showCustomShaderProperties =
                    EditorGUILayout.BeginFoldoutHeaderGroup(showCustomShaderProperties, "Custom Shader Properties");
                if (showCustomShaderProperties)
                {
                    importManager.AlbedoMapKey =
                        EditorGUILayout.TextField("Albedo Map Key", importManager.AlbedoMapKey);
                    importManager.SmoothnessMapKey =
                        EditorGUILayout.TextField("Smoothness Map Key", importManager.SmoothnessMapKey);
                    importManager.NormalMapKey =
                        EditorGUILayout.TextField("Normal Map Key", importManager.NormalMapKey);
                    importManager.EmissionMapKey =
                        EditorGUILayout.TextField("Emission Map Key", importManager.EmissionMapKey);
                    if (!docked)
                    {
                        window.minSize = new Vector2(400, 250);
                        window.maxSize = new Vector2(400, 250);
                    }
                }
                else
                {
                    if (!docked)
                    {
                        window.minSize = new Vector2(400, 160);
                        window.maxSize = new Vector2(400, 160);
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();

            }


            GUILayout.FlexibleSpace();
            if (canStartImport)
            {
                bool buttonWasPressed = GUILayout.Button("Import Assets", GUILayout.Width(position.width),
                    GUILayout.Height(60), GUILayout.ExpandWidth(true));
                if (buttonWasPressed)
                {
                    var path = EditorUtility.OpenFolderPanel("File Importer", "Assets", "");
                    CustomFileUtil.Import(path);
                }
            }
            else
            {
                var style = new GUIStyle();
                style.richText = true;
                //var color = "grey";
                EditorGUILayout.BeginHorizontal(style);
                var r = new Rect(60, 254, 10, 10);
                var message = !importManager.UseCustomAssetPipLine
                    ? "Custom Import Is Disabled\nFor Importing Please Drag The Assets To The Desired Location"
                    : "Please Insert Sub Folder Name";
                EditorGUILayout.HelpBox(new GUIContent(message));
                EditorGUILayout.EndHorizontal();

            }


        }


    }
}
