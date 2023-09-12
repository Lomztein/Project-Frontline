using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class WaypointBase : MonoBehaviour, IWaypoint
{
    public Vector3 Position => transform.position;

    public abstract IWaypoint GetNext();
    public abstract IWaypoint GetPrev();

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(new Ray(transform.position, Waypoint.IncomingVector(this)));
        Gizmos.DrawWireSphere(transform.position + Waypoint.IncomingVector(this), 0.25f);

        Gizmos.DrawRay(new Ray(transform.position, Waypoint.OutgoingVector(this)));
        Gizmos.DrawSphere(transform.position + Waypoint.OutgoingVector(this), 0.5f);
    }


    public virtual bool IsValid()
        => this != null && Waypoint.IsLinked(this);

    private void Awake()
    {
        Waypoint.AddWaypoint(this);
    }

    private void OnDestroy()
    {
        Waypoint.RemoveWaypoint(this);
    }
}
