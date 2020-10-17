using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Waypoint : MonoBehaviour
{
    private static List<Waypoint> _allWaypoints = new List<Waypoint>();

    public Vector3 IncomingVector;
    public Vector3 OutgoingVector;

    public Waypoint NextWaypoint;

    private void Awake()
    {
        if (_allWaypoints != null)
        {
            _allWaypoints.Add(this);
        }
    }

    private void OnDestroy()
    {
        if (_allWaypoints != null)
        {
            _allWaypoints.Remove(this);
        }
    }

    public static Waypoint GetNearest(Vector3 position)
    {
        Waypoint waypoint = null;
        float nearestValue = float.MaxValue;
        foreach (var point in _allWaypoints)
        {
            float dist = Vector3.SqrMagnitude(point.transform.position - position);
            if (dist < nearestValue)
            {
                nearestValue = dist;
                waypoint = point;
            }
        }

        return waypoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(new Ray(transform.position, IncomingVector));
        Gizmos.DrawWireSphere(transform.position + IncomingVector, 0.25f);

        Gizmos.DrawRay(new Ray(transform.position, OutgoingVector));
        Gizmos.DrawSphere(transform.position + OutgoingVector, 0.5f);
    }
}
