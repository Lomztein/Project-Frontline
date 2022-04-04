using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Attribute Weight Tables", menuName = "Unit Weight Tables/Attribute")]
public class AttributeUnitWeightTable : UnitWeightTable
{
    public enum Function { Custom, Damage, Health, Cost, Range, Speed, ProductionTime }
    public Function AttributeGetter;

    private static Func<GameObject, float>[] _funcs =
    {
        null,
        new Func<GameObject, float>(x => x.GetComponent<Unit>().GetWeapons().Sum(x => x.GetDPS()) / ProductionScore(x)),
        new Func<GameObject, float>(x => x.GetComponentsInChildren<Health>().Sum(x => x.MaxHealth) / ProductionScore(x)),
        new Func<GameObject, float>(x => x.GetComponent<Unit>().Cost / ProductionScore(x)),
        RangeScore,
        SpeedScore,
        ProductionScore,
    };

    private Func<GameObject, float> _function;

    private static float RangeScore(GameObject obj)
    {
        AttackerController atc = obj.GetComponent<AttackerController>();
        SentryController sc = obj.GetComponent<SentryController>();
        if (atc)
        {
            return atc.HoldRange;
        }
        else if (sc)
        {
            return sc.AttackRange / 4f; // Since they can't move, they effectively have sagnificantly reduced range. Number is magic LOL
        }
        return 0f;
    }

    private static float SpeedScore(GameObject obj)
    {
        MobileBody mb = obj.GetComponentInChildren<MobileBody>();
        if (mb)
        {
            return mb.MaxSpeed;
        }
        return 0;
    }

    private static float ProductionScore (GameObject go)
    {
        ProductionInfo info = go.GetComponent<ProductionInfo>();
        if (info)
        {
            return info.ProductionTime;
        }
        return 120f; // magic numbers cause fuck you
    }

    protected void SetCustomFunction (Func<GameObject, float> func)
    {
        _function = func;
        AttributeGetter = Function.Custom;
    }

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        var results = new Dictionary<GameObject, float>();
        if (AttributeGetter != Function.Custom)
        {
            _function = _funcs[(int)AttributeGetter];
        }

        float highest = 0f;
        foreach (GameObject go in options)
        {
            float weight = _function(go);
            if (weight > highest)
            {
                highest = weight;
            }
            results.Add(go, weight);
        }

        foreach (GameObject go in options)
        {
            results[go] = results[go] / highest;
        }
        return results;
    }
}
