using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : AttackerController
{
    public Transform SwarmCenter;

    public float WaypointSpeed;
    public float TravelHeight;

    protected override void MoveAlongWaypoints()
    {
        if (_currentWaypoint)
        {
            Vector3 pos = SwarmCenter.position + _currentWaypoint.OutgoingVector * WaypointSpeed * Time.fixedDeltaTime;
            pos.y = TravelHeight;
            SwarmCenter.position = pos;
        }
    }

    protected override void MoveTowardsTarget()
    {
        Vector3 pos = CurrentTarget.GetPosition();
        pos.y = Mathf.Max(TravelHeight, pos.y);
        SwarmCenter.position = pos;
    }
}
