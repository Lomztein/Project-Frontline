using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageMatrix
{
    public enum Damage
    {
        Gun, Cannon, Grenade, Energy, Tesla, Rocket, APGun
    }

    public enum Armor
    {
        Kevlar, Flak, Light, Medium, Heavy, Structure, Shield
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
            { Armor.Flak, 0.4f },
            { Armor.Light, 1.2f },
            { Armor.Medium, 1.5f },
            { Armor.Heavy, 0.8f },
            { Armor.Shield, 0.6f },
        } },
        { Damage.Gun, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 1.0f },
            { Armor.Flak, 1.3f },
            { Armor.Light, 0.7f },
            { Armor.Medium, 0.3f },
            { Armor.Heavy, 0.05f },
            { Armor.Structure, 0.2f },
            { Armor.Shield, 1.3f },
        } },
        { Damage.Rocket, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 0.5f },
            { Armor.Flak, 0.2f },
            { Armor.Light, 1.5f },
            { Armor.Medium, 1.3f },
            { Armor.Heavy, 0.6f },
            { Armor.Structure, 1.2f },
            { Armor.Shield, 0.2f },
        } },
        { Damage.APGun, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 1.3f },
            { Armor.Flak, 1.1f },
            { Armor.Light, 0.8f },
            { Armor.Medium, 0.6f },
            { Armor.Heavy, 0.3f },
            { Armor.Structure, 0.4f },
            { Armor.Shield, 2f },
        } },
        { Damage.Energy, new Dictionary<Armor, float>()
        {
            { Armor.Kevlar, 0.4f },
            { Armor.Flak, 0.2f },
            { Armor.Light, 0.6f },
            { Armor.Medium, 0.8f },
            { Armor.Heavy, 1.5f },
            { Armor.Structure, 1.4f },
            { Armor.Shield, 0.1f },
        } }
    };
}
