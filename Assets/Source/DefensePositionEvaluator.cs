using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class DefensePositionEvaluator : IPositionEvaluator
{
    public float Evaluate(Commander commander, GameObject go, Vector3 position)
    {
        Vector3 differenceDirection = commander.Fortress.forward;

        float difference = VectorUtils.DifferenceAlongDirection(differenceDirection, position, commander.Fortress.position);
        int nearbyStructures = commander.AlivePlaced.Count(x => Vector3.SqrMagnitude(x.transform.position - position) < 100);

        Unit unit = go.GetComponent<Unit>();
        if (unit.Info.UnitType == UnitInfo.Type.Defense)
        {
            return difference / 2f * nearbyStructures;
        }
        return 0f;
    }
}
