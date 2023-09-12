using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamDependentWaypoint : Waypoint
{
    private const string RESOURCE_PATH = "Prefabs/Waypoints/TeamDependentWaypoint";
    public float SearchRange;

    public Team Team;

    public override bool IsValid()
    {
        if (Team)
        {
            return Team.GetCommanders().Any(x => !x.Eliminated);
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (Team == null)
        {
            Team = Team.FindTeam(x => Vector3.Distance(x.transform.position, Position) < SearchRange);
        }
    }

    public static new TeamDependentWaypoint Create(Vector3 position, IWaypoint next, IWaypoint prev)
    {
        GameObject newWaypoint = Instantiate(Resources.Load<GameObject>(RESOURCE_PATH), position, Quaternion.identity);
        TeamDependentWaypoint odw = newWaypoint.GetComponent<TeamDependentWaypoint>();
        odw.Next = next;
        odw.Prev = prev;
        return odw;
    }
}
