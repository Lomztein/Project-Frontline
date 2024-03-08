using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DamageModifierUtils
{
    private const float VERSUS_THRESHOLD = 0.2f;
    private const int VERSUS_MIN = 1;
    private const int VERSUS_MAX = 2;

    private static readonly string[] IGNORE_VERSUS = new string[] { "Mind Control", "Heal", "Anti-Air" };

    public static IEnumerable<DamageModifier> GetHighestAgainst(DamageModifier modifier, IEnumerable<DamageModifier> list)
        => GetVersusAgainst(modifier, list, false);

    public static IEnumerable<DamageModifier> GetLowestAgainst(DamageModifier modifier, IEnumerable<DamageModifier> list)
        => GetVersusAgainst(modifier, list, true);

    private static IEnumerable<DamageModifier> GetVersusAgainst(DamageModifier modifier, IEnumerable<DamageModifier> list, bool sortByDescending)
    {
        list = list.Where(x => !x.IsCreatedAtRuntime() && !IGNORE_VERSUS.Contains(x.Name));

        float threshold = VERSUS_THRESHOLD;
        float[] modifiers = list.Select(x => DamageModifier.Combine(modifier, x)).ToArray();

        DamageModifier[] array = list.ToArray();
        DamageModifier[] copy = new DamageModifier[array.Length];
        Array.Copy(array, copy, array.Length);

        Array.Sort(modifiers, copy);
        if (!sortByDescending)
        {
            threshold = 1f - threshold;
            Array.Reverse(copy);
        }

        var result = copy.Where(x => DamageModifier.Combine(modifier, x) > threshold).ToArray();
        if (!result.Any())
        {
            for (int i = 0; i < Mathf.Min(copy.Length, VERSUS_MIN); i++)
            {
                yield return copy[i];
            }
        }

        for (int i = 0; i < Mathf.Min(result.Length, VERSUS_MAX); i++)
        {
            yield return result[i];
        }
    }

    public static IEnumerable<DamageModifier> GetAvailableArmorModifiers(Commander commander)
        => commander.UnitAvailable.SelectMany(x => x.Key.GetComponentsInChildren<Health>().Select(y => y.Modifier)).Distinct();

    public static IEnumerable<DamageModifier> GetAvailableWeaponModifiers(Commander commander)
        => commander.UnitAvailable.SelectMany(x => x.Key.GetComponent<Unit>().GetWeapons().Select(y => y.Modifier)).Distinct();

    public static IEnumerable<DamageModifier> ReduceToCommonAncestors(IEnumerable<DamageModifier> modifiers)
    {
        // TODO: Implement this when you're smarter.
        return modifiers;
    }
}