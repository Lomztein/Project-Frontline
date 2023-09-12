using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CascadeWaypoint : WaypointBase
{
    private const string RESOURCE_PATH = "Prefabs/Waypoints/CascadeWaypoint";

    public IWaypoint Previous;
    public IWaypoint[] Options;

    public override IWaypoint GetNext()
    {
        foreach (var waypoint in Options)
        {
            if (waypoint.IsValid())
                return waypoint;
        }
        return null;
    }

    public override IWaypoint GetPrev()
        => Previous;

    public static CascadeWaypoint Create(Vector3 position, IWaypoint previous, IWaypoint[] options)
    {
        GameObject newWaypoint = Instantiate(Resources.Load<GameObject>(RESOURCE_PATH), position, Quaternion.identity);
        CascadeWaypoint cw = newWaypoint.GetComponent<CascadeWaypoint>();
        cw.Previous = previous;
        cw.Options = options;
        return cw;
    }
}
