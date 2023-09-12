using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class DamageModifier : ScriptableObject
{
    public DamageModifier Base;

    public string Name;
    public string Description;
    public bool Abstract; // Abstract modifiers are not included in the name printout.

    public uint? ID; // "Compile-time" modifiers could have an ID assigned, used for caching.

    //public bool Cachable; // Should be true only if the output // Implement caching if neccesary, consider assigning each modifier an ID using asset post processing for easy indexing.

    public virtual bool Is(DamageModifier other)
        => IsExactly(other) || (Base != null && Base.Is(other));

    public virtual bool IsExactly(DamageModifier other)
        => this == other;

    public float GetValue(DamageModifier target)
    {
        float baseValue = Base ? Base.GetValue(target) : 1f;
        return baseValue * GetValue_Internal(target);
    }

    protected abstract float GetValue_Internal(DamageModifier target);

    public static DamageModifier One => CreateOne();

    private static DamageModifier CreateOne()
    {
        CascadeDamageModifier one = CreateInstance<CascadeDamageModifier>();
        one.Options = new CascadeDamageModifier.Option[0];
        one.Fallback = 1f;
        return one;
    }


    public static float Combine(DamageModifier target, params DamageModifier[] modifiers)
    {
        float value = 1f;
        foreach (var modifier in modifiers)
        {
            value *= modifier.GetValue(target) * target.GetValue(modifier);
        }
        return value;
    }

    public override string ToString()
    {
        if (Base)
        {
            string bn = Base.ToString();
            if (!string.IsNullOrWhiteSpace(bn))
            {
                return bn + ", " + Name;
            }
        }
        if (Abstract)
            return string.Empty;
        else return Name;
    }
}