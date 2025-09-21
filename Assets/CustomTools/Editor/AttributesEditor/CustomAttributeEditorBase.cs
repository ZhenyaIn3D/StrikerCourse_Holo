using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomAttributeEditorBase : Editor
{
    private List<bool> foldOutExpand = new List<bool>();
    private List<bool> foldOutExpandInternalFields = new List<bool>();
    private FieldInfo[] fields;
    private MethodInfo[] methods;
    private bool haveFeildAttributes;
    private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private void OnEnable()
    {
        Type objectType = target.GetType();

        foreach (var fI in objectType.GetFields(flags))
        {
            if ((ExposeField)fI.GetCustomAttribute(typeof(ExposeField)) != null)
            {
                haveFeildAttributes = true;
            }
        }
        

        fields = objectType.GetFields(flags);
        methods = objectType.GetMethods(flags);
        
        if (foldOutExpand.Count != fields.Length)
        {
            for (var index = 0; index < fields.Length; index++)
            {
                foldOutExpand.Add(false);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (!haveFeildAttributes)
        {
            base.OnInspectorGUI();
        }
        else
        {
            HandleFieldAttribute();
        }
        
        HandleMethodsAttributes();
        
        serializedObject.ApplyModifiedProperties();
        
    }

    private void HandleFieldAttribute()
    {
        for (var index = 0; index < fields.Length; index++)
        {
            if ((fields[index].Attributes & FieldAttributes.Private) == FieldAttributes.Private&&(SerializeField)fields[index].GetCustomAttribute(typeof(SerializeField))==null) continue;
            var exposeAttribute = (ExposeField)fields[index].GetCustomAttribute(typeof(ExposeField));
            if (exposeAttribute != null)
            {
                haveFeildAttributes = true;
                DrawPropertiesOfCurrentType(fields[index].Name);
                DrawExposedAttribute(fields[index], index,exposeAttribute.name);
            }
            else
            {
                DrawPropertiesOfCurrentType(fields[index].Name);
            }
            
        }
    }


    private void HandleMethodsAttributes()
    {
        foreach (MethodInfo method in methods)
        {
            var buttonAttribute = (ButtonAttribute)method.GetCustomAttribute(typeof(ButtonAttribute));
            
            if (buttonAttribute != null)
            {
                
                if (GUILayout.Button(buttonAttribute.buttonLabel))
                {
                    method.Invoke(target, buttonAttribute.value);
                }
            }
        }
    }

    private void DrawExposedAttribute(FieldInfo fI, int index,string attributeName)
    {
        object fValue = fI.GetValue(target);
        if (fValue != null)
        {
            DrawPropertiesOfType(fValue.GetType(), fValue, attributeName, index);
        }
        
    }
    
    
    private void DrawPropertiesOfType(Type type, object value,string title,int index)
    {
        EditorGUI.indentLevel += 2;
        SerializedObject so;
        try
        {
            so = new SerializedObject((UnityEngine.Object) value);
        }
        catch
        {
            EditorGUI.indentLevel -= 2;
            return;
        }
        var fieldInfos = type.GetFields(flags);
        if (fieldInfos.Length == 0)
        {
            EditorGUI.indentLevel -= 2;
            EditorGUILayout.HelpBox($"There are no public or serialized field as part of field with type {type.Name}.The ExposeField Attribute is redundant",MessageType.Error);
            return;
        }
        foldOutExpand[index] = EditorGUILayout.Foldout(foldOutExpand[index], title);
        if (foldOutExpand[index])
        {
            EditorGUI.indentLevel += 2;
            if (foldOutExpandInternalFields.Count == 0)
            {
                foreach (FieldInfo f in fieldInfos)
                {
                    foldOutExpandInternalFields.Add(false);
                }
            }

            for (var j = 0; j < fieldInfos.Length; j++)
            {
                SerializedProperty serializedProperty = so.FindProperty(fieldInfos[j].Name);

                if (serializedProperty != null && serializedProperty.isArray &&
                    serializedProperty.propertyType != SerializedPropertyType.String)
                {
                    var reorderableList = new ReorderableList(serializedProperty.serializedObject, serializedProperty,
                        true, true,
                        true, true);
                    HandleListProperties(reorderableList);
                    reorderableList.drawHeaderCallback = (Rect rect) =>
                    {
                        rect.x -= 40;
                        EditorGUI.LabelField(rect, serializedProperty.name);
                    };

                    foldOutExpandInternalFields[j] =
                        EditorGUILayout.Foldout(foldOutExpandInternalFields[j], serializedProperty.name);
                    if (foldOutExpandInternalFields[j])
                    {
                        reorderableList.DoLayoutList();
                    }
                }
                else if (serializedProperty != null)
                {
                    EditorGUILayout.PropertyField(serializedProperty, true);
                }
            }


            so.ApplyModifiedProperties();
            EditorGUI.indentLevel -= 2;

        }
        EditorGUI.indentLevel -= 2;
    }


    private void DrawPropertiesOfCurrentType(string name)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty(name), true);
    }

    private void HandleListProperties(ReorderableList reorderableList)
    {
        
        
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, rect.height), element, GUIContent.none);
        };
        
        reorderableList.elementHeightCallback = (int index) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element);
        };
        
    }
    
}