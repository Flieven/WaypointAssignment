using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Waypoint))]
public class WaypointInspector : Editor
{
    Waypoint Point;

    protected static bool ShowColorsMenu = false;
    protected static bool ShowLineMenu = false;
    protected static bool ShowGUIMenu = false;
    protected static bool ShowIndexManipulate = false;

    #region SerializedProperties
    private SerializedObject serializedTarget;

    private SerializedProperty isLoop;
    private SerializedProperty listIndex;

    private SerializedProperty lineColor;
    private SerializedProperty gizmoColor;

    private SerializedProperty lineType;
    private SerializedProperty lineSpacing;

    private SerializedProperty fontSize;
    private SerializedProperty gizmoSize;

    private SerializedProperty PointList;
    #endregion

    private void OnEnable()
    {
        Point = (Waypoint)target;

        SceneView.duringSceneGui += DuringSceneGUI;

        serializedTarget = new SerializedObject(Point);

        isLoop = serializedTarget.FindProperty("LoopWaypoints");
        listIndex = serializedTarget.FindProperty("WaypointNumber");

        lineColor = serializedTarget.FindProperty("LineColors");
        gizmoColor = serializedTarget.FindProperty("GizmoColors");

        lineType = serializedTarget.FindProperty("SelectedType");
        lineSpacing = serializedTarget.FindProperty("DottedLineSpacing");

        fontSize = serializedTarget.FindProperty("FontSize");
        gizmoSize = serializedTarget.FindProperty("WaypointGizmoSize");

        PointList = serializedTarget.FindProperty("Waypoints");
    }

    private void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;

    public override void OnInspectorGUI()
    {
        serializedTarget.Update();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Waypoint", GUILayout.Width(120)))
        {
            Point.Add();
        }
        if(GUILayout.Button("Remove Waypoint", GUILayout.Width(120)))
        {
            Point.Remove();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(isLoop);

        ShowIndexManipulate = EditorGUILayout.Foldout(ShowIndexManipulate, "Index-based Insert/Remove");
        if(ShowIndexManipulate)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(listIndex);
            if (GUILayout.Button("Add at Index", GUILayout.Width(120)))
            {
                Point.Insert();
            }
            if (GUILayout.Button("Remove at Index", GUILayout.Width(120)))
            {
                Point.RemoveAt();
            }
            GUILayout.EndHorizontal();
        }

        ShowColorsMenu = EditorGUILayout.Foldout(ShowColorsMenu, "Color Settings");
        if(ShowColorsMenu)
        { 
            EditorGUILayout.PropertyField(lineColor);
            EditorGUILayout.PropertyField(gizmoColor);
        }
        ShowLineMenu = EditorGUILayout.Foldout(ShowLineMenu, "Line Settings");
        if (ShowLineMenu)
        {
            EditorGUILayout.PropertyField(lineType);
            EditorGUILayout.PropertyField(lineSpacing);
        }
        ShowGUIMenu = EditorGUILayout.Foldout(ShowGUIMenu, "GUI Settings");
        if (ShowGUIMenu)
        {
            EditorGUILayout.PropertyField(fontSize);
            EditorGUILayout.PropertyField(gizmoSize);
        }

        EditorGUILayout.PropertyField(PointList);

        serializedTarget.ApplyModifiedProperties();
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        DrawHandles();
        if (Event.current.type == EventType.MouseMove) { sceneView.Repaint(); }
    }

    private void DrawHandles()
    {
        for (int i = 0; i < Point.Waypoints.Count; i++)
        {
            Handles.color = Point.LineColors;
            if (Handles.Button(Point.CalculateMidPoint(i), Quaternion.identity, 1f, 1f, Handles.SphereHandleCap)) { Point.Insert(i); }
        }

        serializedTarget.Update();

        for (int i = 0; i < PointList.arraySize; i++)
        {
            if(i > 0) { PointList.GetArrayElementAtIndex(i).vector3Value = Handles.PositionHandle(Point.Waypoints[i], Quaternion.identity); }
            else { PointList.GetArrayElementAtIndex(i).vector3Value = Point.transform.position; }
        }

        serializedTarget.ApplyModifiedProperties();
    }
}
