using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Waypoint : WaypointBase
{
    private static List<IWaypoint> _allWaypoints = new List<IWaypoint>();
    private const string WAYPOINT_PREFAB = "Prefabs/Waypoints/Waypoint";

    public IWaypoint Next;
    public IWaypoint Prev;

    public static void AddWaypoint(IWaypoint waypoint)
        => _allWaypoints?.Add(waypoint);

    public static void RemoveWaypoint(IWaypoint waypoint)
        => _allWaypoints?.Remove(waypoint);

    public static void ClearWaypoints()
    {
        if (_allWaypoints != null)
        {
            foreach (var waypoint in _allWaypoints)
            {
                if (waypoint is Component comp)
                    Destroy(comp.gameObject);
            }
            _allWaypoints.Clear();
        }
    }

    public static Waypoint Create(Vector3 position, IWaypoint next, IWaypoint prev)
    {
        Waypoint waypoint = Instantiate(Resources.Load<GameObject>(WAYPOINT_PREFAB), position, Quaternion.identity).GetComponent<Waypoint>();
        waypoint.Next = next;
        waypoint.Prev = prev;
        return waypoint;
    }

    public static IWaypoint GetNearest(Vector3 position)
    {
        IWaypoint waypoint = null;
        float nearestValue = float.MaxValue;
        foreach (var point in _allWaypoints)
        {
            float dist = Vector3.SqrMagnitude(point.Position - position);
            if (dist < nearestValue)
            {
                nearestValue = dist;
                waypoint = point;
            }
        }

        return waypoint;
    }

    public override IWaypoint GetNext()
        => Next;

    public override IWaypoint GetPrev()
        => Prev;

    public static Vector3 IncomingVector(IWaypoint waypoint)
    {
        if (IsLinked(waypoint))
        {
            IWaypoint prev = waypoint.GetPrev();
            if (prev == null)
                return OutgoingVector(waypoint) * -1f;
            return (prev.Position - waypoint.Position).normalized;
        }
        throw new InvalidOperationException("Given waypoint is not linked to any other waypoints.");
    }

    public static Vector3 OutgoingVector(IWaypoint waypoint)
    {
        if (IsLinked(waypoint))
        {
            IWaypoint next = waypoint.GetNext();
            if (next == null)
                return IncomingVector(waypoint) * -1f;
            return (next.Position - waypoint.Position).normalized;
        }
        throw new InvalidOperationException("Given waypoint is not linked to any other waypoints.");
    }

    public static bool IsLinked(IWaypoint waypoint)
        => waypoint.GetNext() != null || waypoint.GetPrev() != null;

    public static float IncomingAngle(IWaypoint waypoint)
    {
        Vector3 incoming = IncomingVector(waypoint);
        return Mathf.Atan2(incoming.x, incoming.z) * Mathf.Rad2Deg;
    }

    public static float OutgoingAngle(IWaypoint waypoint)
    {
        Vector3 outgoing = OutgoingVector(waypoint);
        return Mathf.Atan2(outgoing.x, outgoing.z) * Mathf.Rad2Deg;
    }
}
