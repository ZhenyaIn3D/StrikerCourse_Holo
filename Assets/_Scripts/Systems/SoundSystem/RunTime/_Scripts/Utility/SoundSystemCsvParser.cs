// using System.Collections.Generic;
// using System.IO;
// using System.Text;
// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEngine;
//
//
// namespace CustomSoundSystem
// {
//     public class SoundSystemCsvParser
//     {
//
//         private readonly string csvFileName;
//
//         public SoundSystemCsvParser(string csvFileName)
//         {
//             this.csvFileName = csvFileName;
//         }
//
//
//
//
//         [ItemCanBeNull]
//         public string[] ParseCSV()
//         {
//             string folderPath = Path.Combine(SoundSystemReferences.InternalFolderPath, "Resources/Keys");
//             var filePath = $"{folderPath}/{csvFileName}.csv";
//             if (!File.Exists(folderPath))
//             {
//                 Directory.CreateDirectory(folderPath);
//                 AssetDatabase.Refresh();
//             }
//
//             if (!File.Exists(filePath))
//             {
//                 File.WriteAllText(filePath, "");
//                 AssetDatabase.Refresh();
//             }
//
//             TextAsset csv = Resources.Load<TextAsset>("Keys/" + csvFileName);
//             string[] lines = csv.text.Split('\n');
//             var csvData = new string[lines.Length];
//             for (int i = 0; i < lines.Length; i++)
//             {
//                 string[] fields = lines[i].Split(',');
//                 if (fields[0].Equals("")) continue;
//                 csvData[i] = fields[0];
//             }
//
//             return csvData;
//         }
//
//
//         public void UpdateCSVFile(List<string> keys)
//         {
//             string folderPath = Path.Combine(SoundSystemReferences.InternalFolderPath, "Resources/Keys");
//             var filePath = $"{folderPath}/{csvFileName}.csv";
//
//             if (!File.Exists(folderPath))
//             {
//                 Directory.CreateDirectory(folderPath);
//                 AssetDatabase.Refresh();
//             }
//
//             if (!File.Exists(filePath))
//             {
//                 File.WriteAllText(filePath, "");
//                 AssetDatabase.Refresh();
//             }
//
//             StringBuilder csvContent = new StringBuilder();
//             for (var index = 0; index < keys.Count; index++)
//             {
//                 var key = keys[index];
//                 csvContent.AppendLine(key + ",");
//             }
//
//             File.WriteAllText(filePath, csvContent.ToString());
//         }
//
//     }
//
// }
//
