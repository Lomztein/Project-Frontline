using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healer Weight Table", menuName = "Unit Weight Tables/Healer")]
public class HealerUnitWeightTable : UnitWeightTable
{
    public float HealthPerHPSRatio;
    public float DefaultProductionTime = 60f;

    public override Dictionary<GameObject, float> GenerateWeights(IEnumerable<GameObject> options)
    {
        float currentHeals = 0f;
        float currentMaxHealth = 0f;

        var currentUnits = Team.GetTeam(Commander.TeamInfo).GetCommanders().SelectMany(x => x.GetPlacedUnits());

        foreach (Unit u in currentUnits)
        {
            (Unit unit, float productionTime) = GetProducingUnitOrUnit(u);
            Health health = unit.GetComponent<Health>();
            currentMaxHealth += health.MaxHealth / productionTime;

            var weapons = unit.GetWeapons();
            foreach (var weapon in weapons)
            {
                if (weapon.DamageType == DamageMatrix.Damage.Heal)
                {
                    currentHeals += weapon.GetDPSOrOverride() / productionTime;
                }
            }
        }

        var weights = new Dictionary<GameObject, float>();
        float highestHps = 0f;
        foreach (var unit in options)
        {
            var weapons = unit.GetComponent<Unit>().GetWeapons();
            float hps = float.Epsilon;

            foreach (var weapon in weapons)
            {
                if (weapon.DamageType == DamageMatrix.Damage.Heal)
                {
                    hps += weapon.GetDPSOrOverride();
                }
            }
            if (hps > 0.1f)
            {
                weights.Add(unit, (1f - Mathf.Clamp01(currentHeals / (currentMaxHealth / HealthPerHPSRatio))) * hps);
            }
            else
            {
                weights.Add(unit, 0f);
            }

            if (hps > highestHps) highestHps = hps;
        }

        // Normalize by highest HPS
        foreach (var option in options)
        {
            weights[option] /= highestHps;
        }

        return weights;
    }

    private (Unit unit, float prodInfo) GetProducingUnitOrUnit (Unit unit)
    {
        UnitFactory factory = unit.GetComponent<UnitFactory>();
        if (factory)
        {
            return (factory.UnitPrefab.GetComponent<Unit>(), factory.UnitPrefab.GetComponent<ProductionInfo>().ProductionTime);
        }
        return (unit, DefaultProductionTime);
    }
}
