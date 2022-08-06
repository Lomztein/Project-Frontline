using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AggregateUnitWeightTable : UnitWeightTableBase
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
                result[weight.Key] = Aggregate(result[weight.Key], weight.Value);
            }
        }

        float highest = 0f;
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
}
