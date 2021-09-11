using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder
{
    public static Func<Vector3, GameObject, float> DefaultEvaluator { get; private set; } = (center, go) => -Vector3.SqrMagnitude(go.transform.position - center);
    public static Predicate<GameObject> DefaultFilter { get; private set; } = go => true;

    private Func<Vector3, GameObject, float> _evaluator;
    private Predicate<GameObject> _filter;

    public TargetFinder () : this(DefaultEvaluator, DefaultFilter)
    {
    }

    public TargetFinder(Func<Vector3, GameObject, float> evaluator) : this(evaluator, DefaultFilter)
    {
    }

    public TargetFinder(Predicate<GameObject> filter) : this(DefaultEvaluator, filter)
    {
    }

    public TargetFinder (Func<Vector3, GameObject, float> evaluator, Predicate<GameObject> filter)
    {
        _evaluator = evaluator;
        _filter = filter;
    }

    public void SetEvaluator(Func<Vector3, GameObject, float> evaluator) => _evaluator = evaluator;
    public void SetFilter(Predicate<GameObject> filter) => _filter = filter;

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
