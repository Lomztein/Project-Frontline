using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitWeightTable : UnitWeightTableBase
{
    public bool NormalizeOptions = true;
    public bool Cache = true;

    protected Dictionary<GameObject, float> WeightCache = new Dictionary<GameObject, float>();

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        if (Cache)
        {
            WeightCache = GenerateWeights(availableUnits);
        }
    }

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        if (!Cache)
        {
            WeightCache = GenerateWeights(options);
        }

        if (NormalizeOptions)
        {
            var results = new Dictionary<GameObject, float>();

            float highest = 0f;
            foreach (GameObject obj in options)
            {
                float cur = WeightCache[obj];
                if (cur > highest)
                    highest = cur;
            }

            foreach (GameObject obj in options)
            {
                float cur = WeightCache[obj];
                results.Add(obj, cur / highest);
            }

            return results;
        }
        else
        {
            var results = new Dictionary<GameObject, float>();
            foreach (var option in options)
            {
                results.Add(option, WeightCache[option]);
            }
            return results;
        }
    }

    public abstract Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options);

    public override UnitWeightTableBase DeepCopy()
    {
        UnitWeightTable copy = Instantiate(this);
        return copy;
    }
}
