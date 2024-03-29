using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healer Weight Table", menuName = "Unit Weight Tables/Healer")]
public class HealerUnitWeightTable : UnitWeightTable
{
    public float HealthPerHPSRatio;
    public float DefaultProductionTime = 60f;

    public string[] HealerUnitTags = new string[] { "Heal", "Repair" };

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

            if (HealerUnitTags.Any(x => unit.Info.Tags.Contains(x)))
            {
                foreach (var weapon in weapons)
                {
                    currentHeals += weapon.GetDPSOrOverride() / productionTime;
                }
            }
        }

        var weights = new Dictionary<GameObject, float>();
        float highestHps = 0f;
        foreach (var unit in options)
        {
            Unit u = unit.GetComponent<Unit>();
            if (HealerUnitTags.Any(x => u.Info.Tags.Contains(x)))
            {
                var weapons = u.GetWeapons();
                float hps = float.Epsilon;

                foreach (var weapon in weapons)
                {
                    hps += weapon.GetDPSOrOverride();
                }
                if (hps > 0.1f)
                {
                    weights.Add(unit, (1f - Mathf.Clamp01(currentHeals / (currentMaxHealth / HealthPerHPSRatio))) * hps);
                }

                if (hps > highestHps) highestHps = hps;
            }
            else
            {
                weights.Add(unit, 0f);
            }
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
