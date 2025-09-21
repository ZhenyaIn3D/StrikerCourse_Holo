using System;
using CustomTools.Settings;
using UnityEditor;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace CustomTools.SceneManager
{

    [InitializeOnLoad]
    public class AutoSaveScene
    {
        private static double saveIntervals;
        private static double lastSaveTime = 0;
        private static bool useAutoSave => EditorSettingManager.GetData("Use Auto Save")[0] == 'Y';
        private static bool alreadyInAutoSave;

        static AutoSaveScene()
        {
            ToggleAutoSave();
        }

        public static bool UseAutoSave
        {
            get => useAutoSave;

            set
            {
                EditorSettingManager.UpdateData("Use Auto Save", value ? "Yes" : "No");
                ToggleAutoSave();
            }
        }

        public static double SaveIntervals
        {
            get
            {
                if (double.TryParse(EditorSettingManager.GetData("Save Intervals"), out saveIntervals) == false)
                {
                    Debug.LogError("Could not change the save interval");
                }

                if (Math.Abs((lastSaveIntervals - saveIntervals)) > 0.001f)
                {
                    lastSaveIntervals = saveIntervals;
                    UseAutoSave = false;
                    UseAutoSave = true;
                }
                return saveIntervals;
            }
            set
            {
                EditorSettingManager.UpdateData("Save Intervals", value.ToString());
                saveIntervals = SaveIntervals;
            }
        }

        public static double lastSaveIntervals;


        private static async void AutoSave()
        {
            lastSaveTime = EditorApplication.timeSinceStartup;
            while (useAutoSave)
            {
                if (EditorApplication.timeSinceStartup > lastSaveTime + SaveIntervals)
                {
                    lastSaveTime = EditorApplication.timeSinceStartup;
                    CustomEditorSceneManager.SaveScene();
                }
                EditorTitleController.TimeUntilSave =
                    ((int)((lastSaveTime + SaveIntervals) - EditorApplication.timeSinceStartup)).ToString();
                EditorTitleController.Update();
                await Task.Delay(1000);
            }
            
            EditorTitleController.Update();
        }

        private static async void ToggleAutoSave()
        {
            await Task.Delay(200);
            if (!alreadyInAutoSave && useAutoSave)
            {
                AutoSave();
                alreadyInAutoSave = true;
            }

            if (!useAutoSave && alreadyInAutoSave)
            {
                alreadyInAutoSave = false;
            }

        }

        public static double GetTimeUntilSave(int sceneTime)
        {
            return lastSaveTime = sceneTime;
        }

        private double ConvertStringToInt()
        {
            if (double.TryParse(EditorSettingManager.GetData("Save Intervals"), out saveIntervals) == false)
            {
                Debug.LogError("Could not change the save interval");
            }

            return saveIntervals;
        }

    }

}
