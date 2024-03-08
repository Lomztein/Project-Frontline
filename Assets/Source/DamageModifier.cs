using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public abstract class DamageModifier : ScriptableObject
{
    public static IEnumerable<DamageModifier> DamageModifiers => GetEnumerableCache(ref _damageModifiers, "DamageModifiers/Weapons");
    public static IEnumerable<DamageModifier> HealthModifiers => GetEnumerableCache(ref _armorModifiers, "DamageModifiers/Health");

    private static DamageModifier[] _damageModifiers;
    private static DamageModifier[] _armorModifiers;

    public DamageModifier Base;

    public string Name;
    public string Description;

    public int ID; // "Compile-time" modifiers could have an ID assigned, used for caching.

    //public bool Cachable; // Should be true only if the output // Implement caching if neccesary, consider assigning each modifier an ID using asset post processing for easy indexing.

    public virtual bool Is(DamageModifier other)
        => IsExactly(other) || (Base != null && Base.Is(other));

    public virtual bool IsExactly(DamageModifier other)
        => Name == other.Name;

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
        => Name;

    private static T[] GetEnumerableCache<T>(ref T[] cacheObj, string resourcePath) where T : UnityEngine.Object
    {
        if (cacheObj == null)
            cacheObj = Resources.LoadAll<T>(resourcePath);
        return cacheObj;
    }

    public override bool Equals(object other)
    {
        if (other is DamageModifier modifier)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(modifier.Name))
            {
                throw new InvalidOperationException("Modifier name cannot be empty, as they are used for identification.");
            }
            return modifier.Name == Name;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public bool IsCreatedAtRuntime()
        => ID == int.MinValue;

}