
using UnityEditor;
using UnityEngine;

namespace CustomTools.SceneManager
{
    public class CustomSceneManagerWindow : EditorWindow
    {
        private Vector2 scrollPos = Vector2.zero;
        private int sceneMode;

        string[] sceneModesNames = new string[]
        {
            "Single",
            "Additive"
        };

        int[] SceneModeValue = new int[]
        {
            0,
            1
        };

        [MenuItem("CustomTools/Scene Manager %w")]
        public static void ShowWindow()
        {
            var window = GetWindow<CustomSceneManagerWindow>("Scene Manager", true);
            window.minSize = new Vector2(400, 20);
            window.maxSize = new Vector2(400, 200);
            window.SaveChanges();
            window.Show();


        }

        private void OnGUI()
        {
            var autoSave = AutoSaveScene.UseAutoSave;
            autoSave = GUILayout.Toggle(autoSave, "Use Auto Save");
            AutoSaveScene.UseAutoSave = autoSave;

            if (AutoSaveScene.UseAutoSave)
            {
                AutoSaveScene.SaveIntervals =
                    EditorGUILayout.DoubleField("Time Between Saves", AutoSaveScene.SaveIntervals);
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Open Scene Mode");
            sceneMode = EditorGUILayout.IntPopup(sceneMode, sceneModesNames, SceneModeValue);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scenes:");
            EditorGUILayout.EndHorizontal();
            var scenes = CustomEditorSceneManager.GetScenes();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            foreach (var scene in scenes)
            {
                var split = scene.Split('/');
                if (GUILayout.Button(new GUIContent(split[^1])))
                {
                    if (!CustomEditorSceneManager.IsSceneSaved())
                    {
                        var saveScene = EditorUtility.DisplayDialogComplex("Warning",
                            "The Current Scene Is  Saved, Would You Like To Save It?",
                            "Yes", "No", "Cancel");
                        if (saveScene == 0)
                        {
                            CustomEditorSceneManager.SaveScene();
                        }

                        if (saveScene == 2)
                        {
                            EditorGUILayout.EndScrollView();
                            return;
                        }
                    }

                    CustomEditorSceneManager.ChangeScene(scene, sceneMode);
                }

            }

            EditorGUILayout.EndScrollView();

        }

    }

}
