using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CustomTools.Utility;
using UnityEditor;
using UnityEngine;

namespace CustomTools.Settings
{
#if UNITY_EDITOR
    
    [InitializeOnLoad]
    public static class EditorSettingManager
    {
        private static readonly string settingFolderPath = "Assets/CustomTools/Editor/Settings/";
        private static readonly string settingName = "Configuration.csv";
        private static Dictionary<string, string> csvData;
        private static EditorCustomToolsCSVManager _editorCustomToolsCsvManager;

        static EditorSettingManager()
        {
            LoadSettings();
        }

        private static async void LoadSettings()
        {
            var filePath = settingFolderPath + settingName;
            if (!Directory.Exists(settingFolderPath))
            {
                CustomFileUtil.CreateDirectory(settingFolderPath);
                
            }

            if (!File.Exists(filePath))
            {
                await CustomFileUtil.CreateFile(filePath);

            }
            _editorCustomToolsCsvManager = new EditorCustomToolsCSVManager(settingFolderPath, settingName);
            await Task.Delay(100);
            var parsedCsv=_editorCustomToolsCsvManager.ParseCSV();
            csvData = new Dictionary<string, string>(parsedCsv);
        }

        public static string GetData(string key)
        {
            if (_editorCustomToolsCsvManager == null || csvData==null)
            {
                LoadSettings();
            }

            return csvData != null && csvData.ContainsKey(key) ? csvData[key] : AddAndGetData(key);
        }

        public static void UpdateData(string key, string value)
        {
            if (_editorCustomToolsCsvManager == null)
            {
                LoadSettings();
            }

            var dic = _editorCustomToolsCsvManager.ModifyData(key, value);
            foreach (var kvp in dic)
            {
                csvData[kvp.Key] = dic[kvp.Key];
            }
        }

        private static string AddAndGetData(string key)
        {
            if (ReferenceEquals(csvData, null)) return "";
            csvData.Add(key,"No");
            return csvData[key];
        }


    }
    
#endif

}
