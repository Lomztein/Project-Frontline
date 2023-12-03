using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AggregateUnitWeightTable : UnitWeightTableBase, IEnumerable<UnitWeightTableBase>
{
    public List<UnitWeightTableBase> ChildTables;
    public float StartingValue;
    public bool NormalizeOptions = true;

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        var result = new Dictionary<GameObject, float>();
        foreach (var unit in options)
        {
            result.Add(unit, StartingValue);
        }
        foreach (var table in ChildTables)
        {
            var weights = table.GetWeights(options);
            foreach (var weight in weights)
            {
                float value = float.IsNaN(weight.Value) ? 0f : weight.Value;
                result[weight.Key] = Aggregate(result[weight.Key], value);
            }
        }

        float highest = float.Epsilon;
        if (NormalizeOptions)
        {
            foreach (var option in options)
            {
                if (result[option] > highest)
                    highest = result[option];
            }
            foreach (var option in options)
            {
                result[option] /= highest;
            }
            return result;
        }else return result;
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        foreach (var table in ChildTables)
        {
            table.Initialize(commander, availableUnits);
        }
    }

    public override UnitWeightTableBase DeepCopy()
    {
        AggregateUnitWeightTable copy = Instantiate(this);
        copy.ChildTables = ChildTables.Select(x => x.DeepCopy()).ToList();
        return copy;
    }

    protected abstract float Aggregate(float aggregator, float value);

    public IEnumerator<UnitWeightTableBase> GetEnumerator()
    {
        return ((IEnumerable<UnitWeightTableBase>)ChildTables).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)ChildTables).GetEnumerator();
    }

    public override UnitWeightTableBase FindTable(Predicate<UnitWeightTableBase> predicate)
    {
        var b = base.FindTable(predicate);
        if (b != null) return b;

        foreach (var table in ChildTables)
        {
            var res = table.FindTable(predicate);
            if (res) return res;
        }

        return null;
    }
}
