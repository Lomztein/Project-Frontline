using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressDistancePositionEvaluator : IPositionEvaluator
{
    public float Evaluate(Commander commander, GameObject unit, Vector3 position)
    {
        return -Vector3.Distance(commander.Fortress.position, position);
    }
}
