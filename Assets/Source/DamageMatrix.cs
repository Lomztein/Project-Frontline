using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageMatrix
{
    public enum Damage
    {
        Gun, Cannon, Grenade, Energy, Tesla, Rocket, APGun, Heal
    }

    public enum Armor
    {
        Infantry = 0, Light = 2, Medium = 3, Heavy = 4, Structure = 5, Shield = 6
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
            { Armor.Infantry, 0.1f },
            { Armor.Light, 0.7f },
            { Armor.Medium, 1.0f },
            { Armor.Heavy, 0.4f },
            { Armor.Shield, 0.4f },
        } },
        { Damage.Gun, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 1f },
            { Armor.Light, 0.3f },
            { Armor.Medium, 0.1f },
            { Armor.Heavy, 0.05f },
            { Armor.Structure, 0.15f },
            { Armor.Shield, 1.0f },
        } },
        { Damage.Grenade, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 1f },
            { Armor.Light, 0.35f },
            { Armor.Medium, 0.05f },
            { Armor.Heavy, 0f },
            { Armor.Structure, 0.7f },
            { Armor.Shield, 1.0f },
        } },
        { Damage.Rocket, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 0.2f },
            { Armor.Light, 1.0f },
            { Armor.Medium, 0.7f },
            { Armor.Heavy, 0.3f },
            { Armor.Structure, 0.4f },
            { Armor.Shield, 0.14f },
        } },
        { Damage.APGun, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 1.0f },
            { Armor.Light, 0.6f },
            { Armor.Medium, 0.3f },
            { Armor.Heavy, 0.1f },
            { Armor.Structure, 0.4f },
            { Armor.Shield, 1.0f },
        } },
        { Damage.Energy, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 0.5f },
            { Armor.Light, 0.3f },
            { Armor.Medium, 0.6f },
            { Armor.Heavy, 1.0f },
            { Armor.Structure, 0.8f },
            { Armor.Shield, 0.05f },
        } },
        { Damage.Tesla, new Dictionary<Armor, float>()
        {
            { Armor.Infantry, 0.6f },
            { Armor.Light, 0.7f },
            { Armor.Medium, 1.0f },
            { Armor.Heavy, 1.0f },
            { Armor.Structure, 0.3f },
            { Armor.Shield, 1.5f },
        } },
    };

    public static Dictionary<Damage, Dictionary<Armor, float>> CopyMapping() => new Dictionary<Damage, Dictionary<Armor, float>>(_mapping);
}
