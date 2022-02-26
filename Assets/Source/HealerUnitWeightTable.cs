using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healer Weight Table", menuName = "Unit Weight Tables/Healer")]
public class HealerUnitWeightTable : UnitWeightTableBase
{
    public float HealthPerHPSRatio;
    public Vector2 HealerWeightMinMax;
    public float DefaultProductionTime = 60f;

    private Commander _commander;
    private IEnumerable<GameObject> _availableUnits;

    public override UnitWeightTableBase DeepCopy()
    {
        return Instantiate(this);
    }

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        float currentHeals = 0f;
        float currentMaxHealth = 0f;

        var currentUnits = Team.GetTeam(_commander.TeamInfo).GetCommanders().SelectMany(x => x.GetPlacedUnits());
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
                    currentHeals += weapon.GetDPS() / productionTime;
                }
            }
        }

        var weights = new Dictionary<GameObject, float>();
        foreach (var unit in options)
        {
            var weapons = unit.GetComponent<Unit>().GetWeapons();
            bool anyHeals = false;
            foreach (var weapon in weapons)
            {
                if (weapon.DamageType == DamageMatrix.Damage.Heal)
                {
                    weights.Add(unit, Mathf.Lerp(HealerWeightMinMax.x, HealerWeightMinMax.y, currentHeals / (currentMaxHealth * HealthPerHPSRatio)));
                    anyHeals = true;
                    continue;
                }
            }
            if (anyHeals == false)
            {
                weights.Add(unit, 0f);
            }
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

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        _commander = commander;
        _availableUnits = availableUnits;
    }
}
