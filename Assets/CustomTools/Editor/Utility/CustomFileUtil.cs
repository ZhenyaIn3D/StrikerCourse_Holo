using System;
using System.IO;
using System.Threading.Tasks;
using CustomTools.CustomImporter;
using UnityEditor;
using UnityEngine;

namespace CustomTools.Utility
{
#if UNITY_EDITOR
    
    public static class CustomFileUtil
    {
        private const string finalPath = "Assets/InternalAssets/";
        
        
        public static void Import(string path)
        {
            var dirNames = path.Split('/');
            var dirName = dirNames[^1];

            CopyDirectory(path);
        }

        static void CopyDirectory(string sourceDir)
        {
            var internalPath = "Temp";
            CreateFolder(internalPath);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(internalPath, Path.GetFileName(file));
                CopyFile(file, dest);
            }

            foreach (string subDirectory in Directory.GetDirectories(sourceDir))
            {
                string dest = Path.Combine(internalPath, Path.GetFileName(subDirectory));
                CopyDirectory(subDirectory);
            }

            
            AssetDatabase.Refresh();
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
            Debug.Log("Directory created successfully.");
        }

        public static async Task CreateFile(string path)
        {
            await using FileStream fs = File.Create(path);
            Debug.Log("File created successfully.");
        }

        public static void CreateFolder(string folderName)
        {
            if (!Directory.Exists(finalPath + folderName))
            {
                Directory.CreateDirectory(finalPath + folderName);
            }
        }

        public static void CreateFolderInRoot(string folderName)
        {
            var path = "Assets/" + folderName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CopyFile(string file, string fileName)
        {
            File.Copy(file, finalPath + fileName, true);
        }

        public static void MoveModelFromTemp(string modelName, bool useSubFolder = false)
        {
            var split = modelName.Split('/');
            var destFolder = useSubFolder ? split[0] : "";
            var fileName = useSubFolder ? split[^1] : modelName;
            CreateFolder($"Meshes/{destFolder}");
            var dest = $"{finalPath}Meshes/{modelName}.fbx";
            if (!File.Exists(dest))
            {
                //FileUtil.MoveFileOrDirectory($"{finalPath}Temp/{fileName}.fbx", dest);
                FileUtil.CopyFileOrDirectory($"{finalPath}Temp/{fileName}.fbx", dest);
            }
        }

        public static Material CreateMaterial(Material material, string folderName,string subFolder="",string shaderPath = "" ,bool useSubFolder = false,
            bool useCustomShader = false)
        {
            folderName = useSubFolder ? $"{subFolder}/{folderName}" : folderName;
            CreateFolder($"Materials/{folderName}");
            var path = $"{finalPath}/Materials/{folderName}/{material.name}.mat";
            Material mat;
            if (!File.Exists(path))
            {
                if (useCustomShader)
                {
                    var parseShaderPath = shaderPath.Split("Assets");
                    var shader = AssetDatabase.LoadAssetAtPath<Shader>($"Assets{parseShaderPath[^1]}");
                    mat = new Material(shader);
                }
                else
                {
                    var shader = Shader.Find("Standard");
                    mat = new Material(Shader.Find("Standard"));
                }
                
                AssetDatabase.CreateAsset(mat, path);
            }
            
            return (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));
        }

        public static Texture2D CreateTexture(Texture2D tex, string internalPath,string subFolder="", bool useSubFolder = false)
        {
            internalPath = useSubFolder ? $"{subFolder}/{internalPath}" : internalPath;


            CreateFolder($"Textures/{internalPath}");
            var path = $"{finalPath}Textures/{internalPath}/{tex.name}.jpg";
            if (!File.Exists(path))
            {
                FileUtil.CopyFileOrDirectory($"{finalPath}Temp/{tex.name}.jpg", path);
            }

            return tex;
        }

        public static T GetAssetFromPath<T>(string folder, string name)
        {
            var path = "";
            if (typeof(T) == typeof(Texture2D))
            {
                path = $"{finalPath}Textures/{folder}/{name}.jpg";
            }

            if (typeof(T) == typeof(GameObject))
            {
                path = $"{finalPath}Meshes/{name}.fbx";
            }

            return (T)Convert.ChangeType(AssetDatabase.LoadAssetAtPath(path, typeof(T)), typeof(T));
        }

        public static async void CreatePrefab(string modelName, bool useSubFolder = false)
        {
            await Task.Delay(100);
            var split = modelName.Split('/');
            var folderName = useSubFolder ? $"Prefabs/{split[0]}" : "Prefabs";
            var fileName = useSubFolder ? split[1] : modelName;
            CreateFolderInRoot(folderName);
            PrefabUtility.SaveAsPrefabAsset(GetAssetFromPath<GameObject>(folderName, modelName),
                $"Assets/{folderName}/{fileName}.prefab");
        }

        public static void Delete(string folderName)
        {
            AssetDatabase.DeleteAsset(folderName);
        }


    }
    
#endif

}
