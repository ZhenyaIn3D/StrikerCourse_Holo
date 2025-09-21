using System;
using System.Linq;
using UnityEditor;
using System.Reflection;
using CustomTools.SceneManager;

namespace CustomTools
{
    public class EditorTitleController
    {
        
        private EditorTitleController()
        {
        }
        
        
        private static bool alreadyAutoSave;
        private static object title;
        

        public static string TimeUntilSave = "";

        public static void UpdateTitleWrapper()
        {
            Type tEditorApplication = typeof(EditorApplication);
            Type tApplicationTitleDescriptor = tEditorApplication.Assembly.GetTypes()
                .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");
            
            EventInfo eiUpdateMainWindowTitle = tEditorApplication.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo miUpdateMainWindowTitle = tEditorApplication.GetMethod("UpdateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
            
            Type delegateType = typeof(Action<>).MakeGenericType(tApplicationTitleDescriptor);
            MethodInfo methodInfo = ((Action<object>)CustomUpdateTitle).Method;
            Delegate del = Delegate.CreateDelegate(delegateType, null, methodInfo);
            
            eiUpdateMainWindowTitle.GetAddMethod(true).Invoke(null, new object[] { del });
            miUpdateMainWindowTitle.Invoke(null, new object[0]);
            eiUpdateMainWindowTitle.GetRemoveMethod(true).Invoke(null, new object[] { del });
        }

        private static void CustomUpdateTitle(object desc)
        {
            var field = typeof(EditorApplication).Assembly.GetTypes()
                .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor")
                .GetField("title", BindingFlags.Instance | BindingFlags.Public);
            title = field.GetValue(desc);
        }
        
        
        public static void Update()
        {
            Type tEditorApplication = typeof(EditorApplication);
            Type tApplicationTitleDescriptor = tEditorApplication.Assembly.GetTypes()
                .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor");
            
            EventInfo eiUpdateMainWindowTitle = tEditorApplication.GetEvent("updateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo miUpdateMainWindowTitle = tEditorApplication.GetMethod("UpdateMainWindowTitle", BindingFlags.Static | BindingFlags.NonPublic);
            
            Type delegateType = typeof(Action<>).MakeGenericType(tApplicationTitleDescriptor);
            MethodInfo methodInfo = ((Action<object>)CustomUpdateMainWindowTitle).Method;
            Delegate del = Delegate.CreateDelegate(delegateType, null, methodInfo);
            
            eiUpdateMainWindowTitle.GetAddMethod(true).Invoke(null, new object[] { del });
            miUpdateMainWindowTitle.Invoke(null, new object[0]);
            eiUpdateMainWindowTitle.GetRemoveMethod(true).Invoke(null, new object[] { del });
        }

        static void CustomUpdateMainWindowTitle(object desc)
        {
            var autoSave = AutoSaveScene.UseAutoSave;
            var field = typeof(EditorApplication).Assembly.GetTypes()
                .First(x => x.FullName == "UnityEditor.ApplicationTitleDescriptor")
                .GetField("title", BindingFlags.Instance | BindingFlags.Public);
            if (autoSave)
            {
                if (!alreadyAutoSave)
                {
                    title = field.GetValue(desc);
                }
                field.SetValue(desc,  $"{title} {TimeUntilSave}");
            }

            alreadyAutoSave = autoSave;
        }
    }
}