using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetFinder
{
    public static Func<Vector3, GameObject, float> DefaultEvaluator { get; private set; } = (center, go) => -Vector3.SqrMagnitude(go.transform.position - center);
    public static Predicate<GameObject> DefaultFilter { get; private set; } = go => true;

    private List<Func<Vector3, GameObject, float>> _evaluators = new List<Func<Vector3, GameObject, float>>();
    private List<Predicate<GameObject>> _filters = new List<Predicate<GameObject>>();

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
        SetEvaluator(evaluator);
        SetFilter(filter);
    }

    public void SetEvaluator(Func<Vector3, GameObject, float> evaluator)
    {
        _evaluators.Clear();
        _evaluators.Add(evaluator);
    }

    public void AppendEvaluator(Func<Vector3, GameObject, float> evaluator)
        => _evaluators.Add(evaluator);
    public void RemoveEvaluator(Func<Vector3, GameObject, float> evaluator)
        => _evaluators.Remove(evaluator);

    public void SetFilter(Predicate<GameObject> filter)
    {
        _filters.Clear();
        _filters.Add(filter);
    }

    public void AppendFilter(Predicate<GameObject> filter)
        => _filters.Add(filter);
    public void RemoveFilter(Predicate<GameObject> filter)
        => _filters.Remove(filter);

    public GameObject FindTarget (Vector3 center, float range, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(center, range, layerMask);

        GameObject target = null;
        float bestValue = float.MinValue;

        foreach (Collider col in colliders)
        {
            if (_filters.Any(x => x(col.gameObject) == false))
            {
                continue;
            }

            float value = _evaluators.Sum(x => x(center, col.gameObject));
            if (value > bestValue)
            {
                bestValue = value;
                target = col.gameObject;
            }
        }

        return target;
    }
}
