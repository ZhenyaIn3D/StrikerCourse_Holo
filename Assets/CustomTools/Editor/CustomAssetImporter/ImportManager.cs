using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CustomTools.Settings;
using CustomTools.Utility;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomTools.CustomImporter
{
    public class ImportManager
    {
        private Dictionary<string, Material> importedMaterials = new Dictionary<string, Material>();
        private Dictionary<string, Texture2D> importedTextures = new Dictionary<string, Texture2D>();
        private static ImportManager instance = null;


        //private bool useCustomAssetPipeLine =>EditorSettingManager.GetData("Custom Import Pipeline")[0] == 'Y';
        private bool useCustomAssetPipeLine;
        private bool useCustomSubFolder = false;
        private bool useCustomMaterial;
        private string subFolder = "";
        private string shaderPath = "";
        private string albedoMapKey = "";
        private string smoothnessMapKey = "";
        private string normalMapKey = "";
        private string emissionMapKey = "";

        // public bool UseCustomAssetPipLine
        // {
        //     get => useCustomAssetPipeLine;
        //
        //     set => EditorSettingManager.UpdateData("Custom Import Pipeline", value ? "Yes" : "No");
        // }
        
        public bool UseCustomAssetPipLine
        {
            get => useCustomAssetPipeLine;

            set => useCustomAssetPipeLine = value;
        }

        public bool UseCustomSubFolder
        {
            get => useCustomSubFolder;
            set => useCustomSubFolder = value;
        }

        public bool UseCustomMaterial
        {
            get => useCustomMaterial;
            set => useCustomMaterial = value;
        }

        public string SubFolder
        {
            get => subFolder;
            set => subFolder = value;
        }

        public string ShaderPath
        {
            get => shaderPath;
            set => shaderPath = value;
        }

        public string AlbedoMapKey
        {
            get => albedoMapKey;
            set => albedoMapKey = value;
        }

        public string SmoothnessMapKey
        {
            get => smoothnessMapKey;
            set => smoothnessMapKey = value;
        }

        public string NormalMapKey
        {
            get => normalMapKey;
            set => normalMapKey = value;
        }

        public string EmissionMapKey
        {
            get => emissionMapKey;
            set => emissionMapKey = value;
        }



        private ImportManager()
        {
        }

        public static ImportManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImportManager();
                }

                return instance;
            }
        }

        public void OnPreImportModel(ModelImporter modelImporter)
        {
            if (!useCustomAssetPipeLine) return;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            modelImporter.materialSearch = ModelImporterMaterialSearch.Everywhere;
        }

        public void OnPostImportModel(GameObject model)
        {
            if (!useCustomAssetPipeLine) return;
            var modelName = model.name;
            if (useCustomSubFolder)
            {
                modelName = $"{subFolder}/{modelName}";
            }

            CustomFileUtil.MoveModelFromTemp(modelName, useCustomSubFolder);
            CustomFileUtil.CreatePrefab(modelName, useCustomSubFolder);
        }

        public void OnImportMaterials(Material material)
        {
            if (!useCustomAssetPipeLine) return;
            var matName = material.name;
            var mat = CustomFileUtil.CreateMaterial(material, matName,subFolder,shaderPath, useCustomSubFolder, useCustomMaterial);
            importedMaterials.TryAdd(mat.name, mat);
        }

        public void OnImportTexture(Texture2D texture, TextureImporter textureImporter)
        {
            if (!useCustomAssetPipeLine) return;
            var texName = texture.name;
            var splitName = texName.Split('_');
            var modelName = splitName[1];
            var materialName = splitName[2];
            var internalPath = $"{modelName}/{materialName}";
            var texTypeName = splitName[^1];

            if (importedTextures.ContainsKey(texName))
            {
                importedTextures[texName] = CustomFileUtil.CreateTexture(texture, internalPath,subFolder, useCustomSubFolder);
            }
            else
            {
                importedTextures.Add(texName, CustomFileUtil.CreateTexture(texture, internalPath,subFolder, useCustomSubFolder));
            }

            AssignTextureProperties(textureImporter, texTypeName);
        }


        public Material OnAssignMaterial(Material material, Renderer renderer)
        {
            if (!useCustomAssetPipeLine)
            {
                return null;
            }
            

            var mat = importedMaterials[material.name];
            renderer.material = mat;
            return mat;
        }


        public void AssignTextureToMaterial()
        {
            if (!useCustomAssetPipeLine) return;
            foreach (var key in importedTextures.Keys)
            {
                var splitName = key.Split('_');
                var modelName = splitName[1];
                var materialName = splitName[2];
                var internalPath = useCustomSubFolder
                    ? $"{subFolder}/{modelName}/{materialName}"
                    : $"{modelName}/{materialName}";
                var texTypeName = splitName[^1];
                var tex = CustomFileUtil.GetAssetFromPath<Texture2D>(internalPath, key);
              
                importedMaterials[materialName].SetTexture(GetKey(texTypeName), tex);
            }

            CleanAfterImport();
        }

        private void CleanAfterImport()
        {
            importedMaterials.Clear();
            importedTextures.Clear();

            CustomFileUtil.Delete("Assets/InternalAssets/Temp");
        }

        private void AssignTextureProperties(TextureImporter textureImporter, string texType)
        {
            if (!useCustomAssetPipeLine) return;
            textureImporter.textureType =
                texType.Equals("Normal") ? TextureImporterType.NormalMap : TextureImporterType.Default;
        }
        

     


        private string GetKey(string texName)
        {
            if (useCustomMaterial)
            {
                return texName switch
                {
                    "Albedo" => albedoMapKey,
                    "Normal" => normalMapKey,
                    "Smoothness" => smoothnessMapKey,
                    "Emission" => emissionMapKey,
                    _ => albedoMapKey
                };
            }
            
            RenderPipelineAsset renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            if (renderPipelineAsset!=null && renderPipelineAsset.GetType().Name == "UniversalRenderPipelineAsset")
            {
                return texName switch
                {
                    "Albedo" => "_MainTex",
                    "Normal" => "_BumpMap",
                    "Smoothness" => "_MetallicGlossMap",
                    "Emission" => "_EmissionMap",
                    _ => "_MainTex"
                };
            }
            return texName switch
            {
                "Albedo" => "_MainTex",
                "Normal" => "_BumpMap",
                "Smoothness" => "_MetallicGlossMap",
                "Emission" => "_EmissionMap",
                _ => "_MainTex"
            };
        }
    }
    
}

