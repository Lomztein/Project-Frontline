using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AIProfileEditor : EditorWindow
{
    Vector2 scrollPos;
    string t = "This is a string inside a Scroll view!";

    [MenuItem("Project Frontline/AI Profile Editor")]
    public static void OpenWindow ()
    {
        GetWindow<AIProfileEditor>();
    }

    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

        Event current = Event.current;
        Rect clickArea = EditorGUILayout.GetControlRect();
        if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            Debug.Log("Right click!");
        }

        EditorGUILayout.EndScrollView();
    }
}
