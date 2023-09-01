using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Composite Delta Weight Table", menuName = "Unit Weight Tables/Composite Delta")]
public class CompositionDeltaWeightTable : UnitWeightTableBase
{
    public float FallbackProductionTime;
    public float DamageWeight = 1f;
    public float HealthWeight = 1f;

    public string[] SpecialistTags; // Should be ignored as they are handled by other tables.

    private Commander _commander;

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        Dictionary<GameObject, float> results = new Dictionary<GameObject, float>();

        var us = Team.GetTeam(_commander.TeamInfo).GetCommanders();
        var them = Team.GetOtherTeams(_commander.TeamInfo).SelectMany(x => x.GetCommanders());

        var enemyArmor = ComputeTeamArmorProducedPerSecond(them);
        var enemyDamage = ComputeTeamDamageProducedPerSecond(them);
        var ourDamage = ComputeTeamDamageProducedPerSecond(us);

        enemyArmor = SubtractDamageFromArmor(enemyArmor, ourDamage);

        IEnumerable<float> damageScores = Map(options.Select(x => ComputeUnitDamageScore(x, enemyArmor)), 0f, 1f);
        IEnumerable<float> armorScores = Map(options.Select(x => ComputeUnitHealthScore(x, enemyDamage)), 0f, 1f);
        var scores = options.Zip(damageScores, (o, d) => new { Option = o, Damage = d }).Zip(armorScores, (od, a) => new { od.Option, od.Damage, Armor = a });
        
        foreach (var score in scores)
        {
            Unit unit = score.Option.GetComponent<Unit>();
            if (unit.GetWeapons().Any() && !SpecialistTags.Any(x => unit.Info.Tags.Contains(x)))
            {
                float avg = (score.Damage * DamageWeight + score.Armor * HealthWeight) / (DamageWeight + HealthWeight);
                results.Add(score.Option, avg);
            }
            else
            {
                results.Add(score.Option, 0f); // Units without weapons can't really counter anything.
            }
        }

        return results;
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        _commander = commander;
    }

    public override UnitWeightTableBase DeepCopy() => Instantiate(this);

    // Unit damage score indicates how much the units particular damage type(s) is needed at the moment.
    // High score indicates that the unit does high damage against enemy units which are not countered.
    // Score is computed from our current damage produced subtracted from enemy health produced, then multiplied by the weapons DPS against the enemy armor.
    // Scores for all weapons for each enemy armor type is summed. This should result in units whose effective DPS against uncountered units having higher scores.
    private float ComputeUnitDamageScore (GameObject prefab, Dictionary<DamageMatrix.Armor, float> enemyArmor)
    {
        Unit unit = prefab.GetComponent<Unit>();
        float productionTime = GetUnitProductionTime(prefab);
        var weapons = unit.GetWeapons();
        float score = 0f;
        foreach (var weapon in weapons)
        {
            var type = weapon.DamageType;
            float dpsps = weapon.GetDPSOrOverride() / productionTime;
            foreach (var ak in enemyArmor.Keys)
            {
                score += dpsps * DamageMatrix.GetDamageFactor(type, ak) * Mathf.Max(enemyArmor[ak], 0f);
            }
        }
        return score;
    }

    // Unit health score is computed based on the current DPS per second produced by the enemy forces.
    // Higher scores indicate that the particular unit is more resistent to what is produced by the enemy.
    // Computed from unit health multiplied by one minus enemy DPS effeciency against armor.
    // Scores for all healths for each enemy damage type is summed. This should result in armor that is less countered getting higher scores.
    private float ComputeUnitHealthScore (GameObject prefab, Dictionary<DamageMatrix.Damage, float> enemyDamage)
    {
        float productionTime = GetUnitProductionTime(prefab);
        var healths = prefab.GetComponentsInChildren<Health>();
        float score = 0f;
        foreach (var health in healths)
        {
            var type = health.ArmorType;
            float aps = health.MaxHealth / productionTime;
            foreach (var dk in enemyDamage.Keys)
            {
                score += aps * (1-DamageMatrix.GetDamageFactor(dk, type) * enemyDamage[dk]);
            }
        }
        return score;
    }

    private Dictionary<DamageMatrix.Armor, float> SubtractDamageFromArmor(Dictionary<DamageMatrix.Armor, float> armor, Dictionary<DamageMatrix.Damage, float> damage)
    {
        Dictionary<DamageMatrix.Armor, float> result = new Dictionary<DamageMatrix.Armor, float>(armor);
        foreach (var ak in armor.Keys)
        {
            foreach (var dk in damage.Keys)
            {
                float factor = DamageMatrix.GetDamageFactor(dk, ak);
                result[ak] -= factor * damage[dk];
            }
        }
        return result;
    }

    private Dictionary<DamageMatrix.Damage, float> ComputeTeamDamageProducedPerSecond(IEnumerable<Commander> team)
    {
        Dictionary<DamageMatrix.Damage, float> result = NewDamageAccum();
        var types = result.Keys.ToList();
        var factories = team.SelectMany(x => x.AlivePlaced).Select(x => x.GetComponent<UnitFactory>()).Where(x => x != null);
        foreach (var factory in factories)
        {
            GameObject prefab = factory.UnitPrefab;
            Unit unit = prefab.GetComponent<Unit>();
            float productionTime = GetUnitProductionTime(prefab);
            var weapons = unit.GetWeapons();

            foreach (var weapon in weapons)
            {
                var type = weapon.DamageType;
                var dpsps = weapon.GetDPSOrOverride() / productionTime;
                result[type] += dpsps;
            }
        }

        return result;
    }

    private Dictionary<DamageMatrix.Armor, float> ComputeTeamArmorProducedPerSecond(IEnumerable<Commander> team)
    {
        Dictionary<DamageMatrix.Armor, float> result = NewArmorAccum();
        var factories = team.SelectMany(x => x.AlivePlaced).Select(x => x.GetComponent<UnitFactory>()).Where(x => x != null);
        foreach (var factory in factories)
        {
            GameObject prefab = factory.UnitPrefab;
            Unit unit = prefab.GetComponent<Unit>();
            float productionTime = GetUnitProductionTime(prefab);
            var healths = prefab.GetComponentsInChildren<Health>();

            foreach (var health in healths)
            {
                var type = health.ArmorType;
                var hps = health.MaxHealth / productionTime;
                result[type] += hps;
            }
        }

        return result;
    }

    private float GetUnitProductionTime(GameObject unit)
    {
        if (unit.TryGetComponent<ProductionInfo>(out var info))
            return info.ProductionTime;
        return FallbackProductionTime;
    }

    private Dictionary<DamageMatrix.Damage, float> NewDamageAccum ()
        => Enum.GetValues(typeof(DamageMatrix.Damage)).Cast<int>().ToDictionary(x => (DamageMatrix.Damage)x, y => 0f);
    private Dictionary<DamageMatrix.Armor, float> NewArmorAccum()
    => Enum.GetValues(typeof(DamageMatrix.Armor)).Cast<int>().ToDictionary(x => (DamageMatrix.Armor)x, y => 0f);
}
