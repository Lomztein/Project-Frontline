using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New CompositeDamageModifier", menuName = "Damage Modifiers/Composite")]
public class CompositeDamageModifier : DamageModifier
{
    public DamageModifier[] Modifiers;
    public enum AggregationFunction { Min, Max, Average, Multiply }
    public AggregationFunction Function;

    protected override float GetValue_Internal(DamageModifier target)
    {
        if (Modifiers.Length == 0)
            return 0f;

        switch(Function)
        {
            case AggregationFunction.Min:
                return Modifiers.Min(x => x.GetValue(target));
            case AggregationFunction.Max:
                return Modifiers.Max(x => x.GetValue(target));
            case AggregationFunction.Average:
                return Modifiers.Average(x => x.GetValue(target));
            case AggregationFunction.Multiply:
                float agg = 1f;
                foreach (var modifier in Modifiers)
                {
                    agg *= modifier.GetValue(target);
                }
                return agg;
            default:
                throw new System.InvalidOperationException("Function is not valid.");
        }
    }

    public static CompositeDamageModifier CreateFrom(string name, AggregationFunction func, params DamageModifier[] modifiers) 
    {
        CompositeDamageModifier newModifier = CreateInstance<CompositeDamageModifier>();
        newModifier.Name = name;
        newModifier.Modifiers = modifiers;
        newModifier.Function = func;
        return newModifier;
    }
}
