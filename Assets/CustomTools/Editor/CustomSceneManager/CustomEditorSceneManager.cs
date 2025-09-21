using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;


namespace CustomTools.SceneManager
{

    public static class CustomEditorSceneManager
    {
        public static List<string> GetScenes()
        {
            var scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                scenes.Add(scene.path);
            }

            return scenes;
        }


        public static void ChangeScene(string scenePath, int sceneMode)
        {
            EditorSceneManager.OpenScene(scenePath, (OpenSceneMode)sceneMode);
            EditorTitleController.UpdateTitleWrapper();
        }

        public static void SaveScene()
        {
#if UNITY_EDITOR
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
        }

        public static bool IsSceneSaved()
        {
            return !UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty;
        }
    }

}
