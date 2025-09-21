// using System;
// using UnityEditor;
// using UnityEngine;
// using System.Reflection;
//
// [CustomEditor(typeof(MonoBehaviour), true)]
// public class ButtonAttributeEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI(); // Draw the default inspector
//         
//         Type objectType = target.GetType();
//         
//         MethodInfo[] methods = objectType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//         foreach (MethodInfo method in methods)
//         {
//             var buttonAttribute = (ButtonAttribute)method.GetCustomAttribute(typeof(ButtonAttribute));
//             
//             if (buttonAttribute != null)
//             {
//                 
//                 if (GUILayout.Button(buttonAttribute.buttonLabel))
//                 {
//                     method.Invoke(target, buttonAttribute.value);
//                 }
//             }
//         }
//     }
// }