using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitWeightTable : UnitWeightTableBase
{
    public bool NormalizeOptions = true;
    protected Dictionary<GameObject, float> WeightTable = new Dictionary<GameObject, float>();

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        if (NormalizeOptions)
        {
            var results = new Dictionary<GameObject, float>();

            float highest = 0f;
            foreach (GameObject obj in options)
            {
                float cur = WeightTable[obj];
                if (cur > highest)
                    highest = cur;
            }

            foreach (GameObject obj in options)
            {
                float cur = WeightTable[obj];
                results.Add(obj, cur / highest);
            }

            return results;
        }
        else
        {
            var results = new Dictionary<GameObject, float>();
            foreach (var option in options)
            {
                results.Add(option, WeightTable[option]);
            }
            return results;
        }
    }

    protected void SetWeight(GameObject unit, float newWeight)
    {
        if (!WeightTable.ContainsKey(unit))
            WeightTable.Add(unit, newWeight);
        WeightTable[unit] = newWeight;
    }

    public override UnitWeightTableBase DeepCopy()
    {
        UnitWeightTable copy = Instantiate(this);
        copy.WeightTable = new Dictionary<GameObject, float>(WeightTable);
        return copy;
    }
}
