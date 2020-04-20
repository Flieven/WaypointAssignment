using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Collections;

[ExecuteInEditMode]
public class Waypoint : MonoBehaviour
{
    #region Variables
    #region Settings
    #region Size Settings
    [Range(0.2f, 1f)]
    [SerializeField] public float WaypointGizmoSize = 1f;
    [Range(12, 24)]
    [SerializeField] public int FontSize = 18;
    #endregion
    #region ColorSettings
    [SerializeField] public Color GizmoColors = Color.cyan;
    [SerializeField] public Color LineColors = Color.blue;
    #endregion
    #region LineSettings
    [SerializeField] public enum LineType { None, Full, Dotted }
    [SerializeField] public LineType SelectedType = LineType.Full;
    [Range(1f, 10f)]
    [SerializeField] public float DottedLineSpacing = 1f;
    #endregion

    public int WaypointNumber = 0;

    public bool LoopWaypoints;
    #endregion
    public List<Vector3> Waypoints = new List<Vector3>();
    private GUIStyle style = new GUIStyle();

    #endregion

    private void Awake()
    {
        if(Waypoints.Count == 0)
        { Waypoints.Add(transform.position); }
    }

    #region Context Menues
    [ContextMenu("Add Waypoint")]
    public void Add()
    {
        if (Waypoints.Count > 1) { CreatePoint(new Vector3(Waypoints[Waypoints.Count - 1].x + 0.5f, Waypoints[Waypoints.Count - 1].y, Waypoints[Waypoints.Count - 1].z)); }
        else { CreatePoint(new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z)); }
    }
    private void CreatePoint(Vector3 Pos) 
    { 
        Waypoints.Add(Pos); 
    }

    [ContextMenu("Remove Waypoint")]
    public void Remove()
    {
        if(Waypoints.Count > 1) 
        { 
            Waypoints.RemoveAt(Waypoints.Count - 1);
        }
    }

    [ContextMenu("Insert Waypoint At Index")]
    public void Insert()
    {
        Vector3 insertPos = CalculateMidPoint(WaypointNumber);
        WaypointNumber = Mathf.Clamp(WaypointNumber, 0, Waypoints.Count - 1);
        WaypointNumber++;
        Waypoints.Insert(WaypointNumber, insertPos);
    }

    public void Insert(int i)
    {
        Vector3 insertPos = CalculateMidPoint(i);
                int iRef = i;
        iRef++;
        Waypoints.Insert(iRef, insertPos);
    }

    [ContextMenu("Remove Waypoint At Index")]
    public void RemoveAt()
    {
        WaypointNumber = Mathf.Clamp(WaypointNumber, 1, Waypoints.Count - 1);
        Waypoints.RemoveAt(WaypointNumber);
    }

    public Vector3 CalculateMidPoint(int index)
    {
        index = Mathf.Clamp(index, 0, Waypoints.Count - 1);
        if (index == Waypoints.Count - 1)
        {
            if (LoopWaypoints) { return (Waypoints[0] + Waypoints[index]) / 2; }
            else { return new Vector3(Waypoints[Waypoints.Count - 1].x + 0.5f, Waypoints[Waypoints.Count - 1].y, Waypoints[Waypoints.Count - 1].z); }
        }
        else { return ((Waypoints[index] + Waypoints[index + 1]) / 2); }
    }

    #endregion

    #region GUI
    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColors;
        Gizmos.DrawSphere(transform.position, WaypointGizmoSize);

        DrawHandles();
    }

    private void DrawHandles()
    {
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = FontSize;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.Label(transform.position + new Vector3(0, 1.5f, 0), transform.name + " - Waypoint Core", style);

        if (Waypoints.Count > 0)
        {
            for (int i = 0; i < Waypoints.Count; i++)
            {

                if (i < Waypoints.Count -1) 
                { 
                    DrawLine(Waypoints[i], Waypoints[i +1]); 
                }

                if (i == Waypoints.Count - 1 && LoopWaypoints)
                {
                    DrawLine(Waypoints[i], transform.position);
                }

                if (i > 0)
                {
                    Handles.Label(Waypoints[i] + new Vector3(0, 1.5f, 0), transform.name + " - Waypoint " + (i), style);
                    Gizmos.color = GizmoColors;
                    Gizmos.DrawSphere(Waypoints[i], WaypointGizmoSize);
                }
            }
        }
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        Handles.color = LineColors;
        switch (SelectedType)
        {
            case LineType.None:;break;
            case LineType.Full: Handles.DrawLine(start, end); break;
            case LineType.Dotted: Handles.DrawDottedLine(start, end, DottedLineSpacing); break;
        }
    }
    #endregion
}