using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitWeightTableBase : ScriptableObject
{
    public abstract Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options);

    public abstract void Initialize(Commander commander, IEnumerable<GameObject> availableUnits);

    public abstract UnitWeightTableBase DeepCopy();

    public static IEnumerable<float> Normalize(IEnumerable<float> values)
    {
        float max = values.Max();
        return values.Select(x => x / max);
    }

    public static IEnumerable<float> Map(IEnumerable<float> values, float min, float max) 
    {
        if (values.Any())
        {
            float oMin = values.Min();
            float oMax = values.Max();
            IEnumerable<float> ts = values.Select(x => Mathf.InverseLerp(oMin, oMax, x));
            return ts.Select(x => Mathf.Lerp(min, max, x));
        }
        else return values;
    }

    public virtual UnitWeightTableBase FindTable(Predicate<UnitWeightTableBase> predicate)
    {
        if (predicate(this))
            return this;
        return null;
    }
}
