using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace CustomTools.CustomImporter
{
    public class AssetPostprocessorWrapper : AssetPostprocessor
    {
        void OnPreprocessModel()
        {
            ModelImporter importer = (ModelImporter)assetImporter;
            ImportManager.Instance.OnPreImportModel(importer);
        }

        private void OnPostprocessModel(GameObject g)
        {
            ImportManager.Instance.OnPostImportModel(g);
        }

        private void OnPostprocessMaterial(Material material)
        {
            ImportManager.Instance.OnImportMaterials(material);
        }

        private Material OnAssignMaterialModel(Material material, Renderer renderer)
        {
            return ImportManager.Instance.OnAssignMaterial(material, renderer);
        }


        private void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            ImportManager.Instance.OnImportTexture(texture, importer);
        }

        private static async void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            await Task.Delay(300);
            ImportManager.Instance.AssignTextureToMaterial();
        }
    }

}
