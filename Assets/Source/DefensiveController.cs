using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveController : AttackerController
{
    protected override void MoveAlongWaypoints()
    {
        Controllable.Accelerate(0);
        Controllable.Turn(0);
    }
}
