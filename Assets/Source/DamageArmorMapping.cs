﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageArmorMapping
{
    public enum Damage
    {
        Gun, Cannon, Grenade, Fire, Energy, Tesla
    }

    public enum Armor
    {
        Kevlar, Flak, Light, Medium, Heavy, Structure
    }

    private static float _fallbackDamageFactor = 1f;

    public static float GetDamageFactor (Damage damage, Armor armor)
    {
        if (_mapping.TryGetValue(damage, out var armorTable))
        {
            if (armorTable.TryGetValue(armor, out float value))
            {
                return value;
            }
        }
        return _fallbackDamageFactor;
    }

    private static Dictionary<Damage, Dictionary<Armor, float>> _mapping = new Dictionary<Damage, Dictionary<Armor, float>>
    {
        { Damage.Cannon, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 0.7f },
            { Armor.Flak, 0.2f },
            { Armor.Light, 2f },
            { Armor.Medium, 1.5f },
            { Armor.Heavy, 1.2f },
        } },
        { Damage.Gun, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 0.3f },
            { Armor.Flak, 0.7f },
            { Armor.Light, 0.7f },
            { Armor.Medium, 0.3f },
            { Armor.Heavy, 0.05f },
            { Armor.Structure, 0.2f },
        } }
    };
}