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
        if (PrevNode && NextNode)
        {
            Vector3 pos = SwarmCenter.position + Navigation.OutgoingVector(PrevNode, NextNode) * WaypointSpeed * Time.fixedDeltaTime;
            pos.y = TravelHeight;
            SwarmCenter.position = pos;
        }
    }

    protected override void MoveTowardsTarget()
    {
        Vector3 pos = CurrentTarget.GetCenter();
        //pos.y = Mathf.Max(TravelHeight, pos.y);
        SwarmCenter.position = pos;
    }
}
