using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomSoundSystem.Editor
{
    public static class SoundSystemTextureUtility
    {
        public static void ConvertTextureToJson(List<TextureData> data, string jsonPath)
        {
            if (!string.IsNullOrEmpty(jsonPath))
            {
                TextureDataListWrapper wrapper = new TextureDataListWrapper(data);
                string json = JsonUtility.ToJson(wrapper, true);
                File.WriteAllText(jsonPath, json);
                AssetDatabase.Refresh();
                Debug.Log("Texture data saved as JSON: " + jsonPath);
            }
        }

        public static void LoadTexturesFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            TextureDataListWrapper wrapper = JsonUtility.FromJson<TextureDataListWrapper>(json);

            foreach (TextureData textureData in wrapper.textures)
            {
                byte[] textureBytes = System.Convert.FromBase64String(textureData.base64EncodedImage);
                string directoryPath = Path.GetDirectoryName(jsonPath);
                var splitPath = directoryPath.Split("Assets");
                var relativePath = "Assets" + splitPath[1];
                string assetPath = Path.Combine(relativePath, textureData.textureName + ".png");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllBytes(assetPath, textureBytes);
                AssetDatabase.ImportAsset(assetPath);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                Debug.Log("Texture created from JSON: " + assetPath);
            }
        }


        public static void LoadTexturesFromJson(string[] encode, string jsonPath, string[] textureNames)
        {
            for (var i = 0; i < encode.Length; i++)
            {
                var t = encode[i];
                byte[] textureBytes = System.Convert.FromBase64String(t);
                string directoryPath = Path.GetDirectoryName(jsonPath);
                var splitPath = directoryPath.Split("Assets");
                var relativePath = "Assets" + splitPath[1];
                string assetPath = Path.Combine(relativePath, textureNames[i] + ".png");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllBytes(assetPath, textureBytes);
                AssetDatabase.ImportAsset(assetPath);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
                Debug.Log("Texture created from JSON: " + assetPath);
            }
        }


        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {

                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }

    [Serializable]
    public class TextureData
    {
        public Texture2D texture;
        public string textureName;
        public string base64EncodedImage;


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || this.GetType() != obj.GetType()) return false;
            TextureData other = (TextureData)obj;
            return textureName == other.textureName;
        }

        public override int GetHashCode()
        {
            return textureName.GetHashCode();
        }


        public static bool operator ==(TextureData left, TextureData right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return left.textureName == right.textureName;
        }

        public static bool operator !=(TextureData left, TextureData right)
        {
            return left.textureName != right.textureName;
        }

    }

    [Serializable]
    public class TextureDataListWrapper
    {
        public List<TextureData> textures;

        public TextureDataListWrapper(List<TextureData> textures)
        {
            this.textures = textures;
            foreach (TextureData textureData in this.textures)
            {
                if (textureData.texture != null)
                {
                    byte[] textureBytes = textureData.texture.EncodeToPNG();
                    textureData.base64EncodedImage = Convert.ToBase64String(textureBytes);
                    textureData.textureName = textureData.texture.name;
                }
            }
        }
    }
}
