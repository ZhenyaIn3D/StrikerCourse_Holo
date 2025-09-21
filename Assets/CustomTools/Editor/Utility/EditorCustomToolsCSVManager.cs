using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CustomTools.Utility
{
    
    public class EditorCustomToolsCSVManager
    {
        private Dictionary<string, string> csvData = new Dictionary<string, string>();
        private readonly string resourcesFolder;
        private readonly string csvFileName;


        public EditorCustomToolsCSVManager(string resourcesFolder, string csvFileName)
        {
            this.resourcesFolder = resourcesFolder;
            this.csvFileName = csvFileName;
        }
        

        public Dictionary<string, string> ParseCSV()
        {

            string fullPath = Path.Combine(resourcesFolder, csvFileName);
            TextAsset csv = (TextAsset)AssetDatabase.LoadAssetAtPath(fullPath, typeof(TextAsset));
            string[] lines = csv.text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');
                if (fields[0].Equals("")) break;
                if(csvData.ContainsKey(fields[0])) break;
                csvData.Add(fields[0], fields[1]);
            }

            return csvData;
        }


        public Dictionary<string, string> ModifyData(string key, string value)
        {
            if (csvData.ContainsKey(key))
            {
                csvData[key] = value;
            }
            else
            {
                csvData.Add(key,value);
            }

            UpdateCSVFile();

            return csvData;
        }

        private void UpdateCSVFile()
        {
            string filePath = Path.Combine(resourcesFolder, csvFileName);

            if (!File.Exists(filePath))
            {
                Debug.LogError("CSV file not found: " + filePath);
                return;
            }

            StringBuilder csvContent = new StringBuilder();
            foreach (var pair in csvData)
            {
                csvContent.AppendLine(pair.Key + "," + pair.Value);
            }

            File.WriteAllText(filePath, csvContent.ToString());
        }

    }

}