using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class ShieldProjectorPositionEvaluator : IPositionEvaluator
{
    private DefensePositionEvaluator _defenseEvaluator = new DefensePositionEvaluator();

    public float Evaluate(Commander commander, GameObject unit, Vector3 position)
    {
        Transform shield = ShieldUtils.GetShieldInObj(unit);
        if (shield == null) return 0f;

        var others = commander.AlivePlaced.Select(x => ShieldUtils.GetShieldInObj(x.gameObject)).Where(x => x != null);
        var nearbyStructures = commander.AlivePlaced.Where(x => Vector3.SqrMagnitude(position - x.transform.position) < Mathf.Pow(ShieldUtils.ComputeShieldRadius(shield), 2f));
        int count = nearbyStructures.Sum(x => others.Count(y => Vector3.SqrMagnitude(x.transform.position - y.transform.position) < Mathf.Pow(ShieldUtils.ComputeShieldRadius(y), 2f)));

        return (-count * 5f) + _defenseEvaluator.Evaluate(commander, unit, position);
    }
}
