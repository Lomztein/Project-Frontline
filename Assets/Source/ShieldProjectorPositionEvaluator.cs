using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldProjectorPositionEvaluator : IPositionEvaluator
{
    private DefensePositionEvaluator _defenseEvaluator = new DefensePositionEvaluator();

    public float Evaluate(Commander commander, GameObject unit, Vector3 position)
    {
        ShieldProjector projector = unit.GetComponentInChildren<ShieldProjector>();
        if (projector == null) return 0f;

        var others = commander.AlivePlaced.Select(x => x.GetComponentInChildren<ShieldProjector>()).Where(x => x != null);
        var nearbyStructures = commander.AlivePlaced.Where(x => Vector3.SqrMagnitude(position - x.transform.position) < Mathf.Pow(projector.ShieldSize / 2f, 2f));
        int count = nearbyStructures.Sum(x => others.Count(y => Vector3.SqrMagnitude(x.transform.position - y.transform.position) < Mathf.Pow(y.ShieldSize / 2f, 2f)));

        return (-count * 5f) + _defenseEvaluator.Evaluate(commander, unit, position);
    }
}
