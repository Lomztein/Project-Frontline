using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder
{
    private static Func<Vector3, GameObject, float> _defaultEvaluator = (center, go) => -Vector3.SqrMagnitude(go.transform.position - center);
    private static Predicate<GameObject> _defaultFilter = go => true;

    private Func<Vector3, GameObject, float> _evaluator;
    private Predicate<GameObject> _filter;

    public TargetFinder () : this(_defaultEvaluator, _defaultFilter)
    {
    }

    public TargetFinder(Func<Vector3, GameObject, float> evaluator) : this(evaluator, _defaultFilter)
    {
    }

    public TargetFinder(Predicate<GameObject> filter) : this(_defaultEvaluator, filter)
    {
    }

    public TargetFinder (Func<Vector3, GameObject, float> evaluator, Predicate<GameObject> filter)
    {
        _evaluator = evaluator;
        _filter = filter;
    }

    public GameObject FindTarget (Vector3 center, float range, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(center, range, layerMask);

        GameObject target = null;
        float bestValue = float.MinValue;

        foreach (Collider col in colliders)
        {
            if (_filter(col.gameObject) == false)
            {
                continue;
            }

            float value = _evaluator(center, col.gameObject);
            if (value > bestValue)
            {
                bestValue = value;
                target = col.gameObject;
            }
        }

        return target;
    }
}
