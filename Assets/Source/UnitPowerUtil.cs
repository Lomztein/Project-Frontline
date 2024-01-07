using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnitPowerUtil
{
    private const float DPS_MULT = 1f;
    private const float HEALTH_MULT = 0.1f;
    private const float RANGE_MULT = 1f;
    private const float BASE_PRODUCTION_TIME = 40f;
    private const float FALLBACK_PRODUCTION_TIME = 40f;

    public static float ComputeUnitPower(Unit unit)
    {
        float productionTime = FALLBACK_PRODUCTION_TIME;
        if (unit.TryGetComponent(out ProductionInfo info))
        {
            productionTime = info.ProductionTime;
        }
        float productionFactor = BASE_PRODUCTION_TIME / productionTime;

        return
            (ComputeDPSScore(unit) * DPS_MULT
            + ComputeSurvivabilityScore(unit) * HEALTH_MULT)
            * ComputeRangeScore(unit)
            * productionFactor;
    }

    private static float ComputeDPSScore(Unit unit)
    {
        float dps = unit.GetWeapons().Sum(x => ComputeWeaponScore(x));
        return dps;
    }

    private static float ComputeWeaponScore(IWeapon weapon)
    {
        float result = 0f;
        float dps = weapon.GetDPSOrOverride();
        DamageModifier mod = weapon.Modifier;
        foreach (var armorMod in DamageModifier.HealthModifiers)
        {
            result += DamageModifier.Combine(armorMod, mod) * dps;
        }
        return result;
    }

    private static float ComputeSurvivabilityScore(Unit unit)
    {
        Health[] healths = unit.GetComponentsInChildren<Health>();
        return healths.Sum(x => ComputeHealthScore(x));
    }

    private static float ComputeHealthScore(Health health)
    {
        float result = 0f;
        float maxHealth = health.MaxHealth;
        DamageModifier mod = health.Modifier;
        foreach (var damageModifier in DamageModifier.DamageModifiers)
        {
            result += DamageModifier.Combine(damageModifier, mod) * maxHealth;
        }
        return result;
    }

    private static float ComputeRangeScore(Unit unit)
    {
        float range = GetUnitRange(unit);
        return Mathf.Log(range);
    }

    private static float GetUnitRange(Unit unit)
    {
        AIController[] controllers = unit.GetComponentsInChildren<AIController>();
        float sum = 0f;

        foreach (var controller in controllers)
        {
            if (controller is AttackerController attacker)
            {
                sum += attacker.HoldRange;
            }
            else
            {
                sum += controller.AttackRange;
            }
        }
        return sum / controllers.Length;
    }
}
