using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTraversal : MonoBehaviour
{
    [SerializeField] private GameObject WaypointCore = null;
    private Waypoint WaypointRef = null;
    [SerializeField] [ReadOnly] private int currentPoint = 0;
    [SerializeField] [ReadOnly] private int traversalDir = 1;

    private enum Traversal { None, Travers, PingPong }
    [SerializeField] private Traversal TraversalType = Traversal.None;

    private void Awake()
    {
        if(WaypointCore.GetComponent<Waypoint>() != null) 
        { 
            WaypointRef = WaypointCore.GetComponent<Waypoint>();
        }
        else { throw new MissingComponentException("No Waypoint Component in given Waypoint Core in " + transform.name); }
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, WaypointRef.Waypoints[currentPoint]) > 0.1f) { transform.position = Vector3.Lerp(transform.position, WaypointRef.Waypoints[currentPoint], Time.deltaTime);}
        else if( traversalDir == 1 && currentPoint < WaypointRef.Waypoints.Count -1) { currentPoint += traversalDir; }
        else if (traversalDir == -1 && currentPoint > 0) { currentPoint += traversalDir; }
        else { CheckEndTraversal(); }
    }

    private void CheckEndTraversal()
    {
        if(WaypointRef.LoopWaypoints) { currentPoint = 0; }
        else if(TraversalType == Traversal.PingPong) { traversalDir *= -1; }
        else { Debug.LogWarning("Traversal Aborted: No traversal possible"); }
    }
}
