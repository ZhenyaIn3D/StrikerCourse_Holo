using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;

static class ShowHovered
{
    private static GameObject[] viewedGOS;
    private static Vector2 mousePos = Vector3.zero;
    private static Vector2 startPoint = Vector3.zero;
    private static Vector2 endPoint = Vector3.zero;
    private static Rect windowRect;
    private static Rect cropRect;
    private static bool showOptionBox;
    private static bool showCropBox;
    private static bool startDrag= true;
    [InitializeOnLoadMethod]
    static void Init()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView view)
    {
        Handles.BeginGUI();

        if (!showOptionBox && showCropBox)
        {
            float width = endPoint.x - startPoint.x;
            float height = endPoint.y - startPoint.y;
            cropRect = new Rect(startPoint.x, startPoint.y, width, height);

            GUILayout.BeginArea(cropRect, EditorStyles.helpBox);  // Use a simple box style to contain the overlay
            GUILayout.Label("Crop", EditorStyles.boldLabel);  // Simple label in the overlay
            GUILayout.EndArea();
        }
        else if(showOptionBox && !showCropBox)
        {
            windowRect = new Rect(cropRect.x+cropRect.width/2, cropRect.y+cropRect.height/2, 200, viewedGOS!=null?viewedGOS.Length*21+20:20);
            GUILayout.BeginArea(windowRect, EditorStyles.textArea);  // Use a simple box style to contain the overlay

            GUILayout.Label("GO", EditorStyles.boldLabel);  // Simple label in the overlay
            
            if(viewedGOS==null) return;
            foreach (var go in viewedGOS)
            {
                if (GUILayout.Button(go.name))  // Button for interaction
                {
                    Selection.activeGameObject = go;    
                }
            }
      
            GUILayout.EndArea();
        }

        Handles.EndGUI();
        
        
        
        
        
        var evt = Event.current;
        var pickingControlId = GUIUtility.GetControlID(FocusType.Passive);
        
        switch (evt.type)
        {
            case EventType.MouseDown:
            {
                // Make sure to check Tools.viewToolActive before consuming a mouse event, otherwise Scene navigation
                // controls will not work.
                
                if (evt.button==0 && evt.control && startDrag)
                {
                    mousePos = evt.mousePosition;
                    viewedGOS = null;
                    GUIUtility.hotControl = pickingControlId;
                    showOptionBox = false;
                    endPoint = evt.mousePosition;
                    startPoint = evt.mousePosition;
                    startDrag = false;
                    showCropBox = true;
                    
                    evt.Use();
                }
                if ( (evt.button==0 || evt.button==1)&&showOptionBox)
                {
                     if(windowRect.Contains(evt.mousePosition)) return;
                     showCropBox = false;
                     showOptionBox = false;
                     evt.Use(); 
                }
                break;
            }
            
            case EventType.MouseDrag:
            {
                if (evt.button == 0 && evt.control&& !startDrag)
                {
                    GUIUtility.hotControl = pickingControlId;
                    endPoint = evt.mousePosition;
                    evt.Use();
                }
                break;
            }

            case EventType.MouseUp:
            {
                if (!startDrag)
                {
                    if(Vector2.Distance(evt.mousePosition,mousePos)<0.01f) return;
                    viewedGOS= HandleUtility.PickRectObjects(cropRect);
                    showCropBox = false;
                    startDrag = true;
                    showOptionBox = true;
                    evt.Use();
                }
                break;
            }
            
        }
        
    }
}