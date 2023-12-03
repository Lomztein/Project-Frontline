using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Composite Delta Weight Table", menuName = "Unit Weight Tables/Composite Delta")]
public class CompositionDeltaWeightTable : UnitWeightTableBase
{
    public float FallbackProductionTime;
    public float DamageWeight = 1f;
    public float HealthWeight = 1f;
    public float CostWeight = 1f;
    public float RangeWeight = 1f;

    public string[] SpecialistTags; // Should be ignored as they are handled by other tables.

    private Commander _commander;
    public string DebugText;

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        Dictionary<GameObject, float> results = new Dictionary<GameObject, float>();

        var us = Team.GetTeam(_commander.TeamInfo).GetCommanders();
        var them = Team.GetOtherTeams(_commander.TeamInfo).SelectMany(x => x.GetCommanders());

        var enemyArmor = ComputeTeamArmorProducedPerSecond(them);
        var enemyDamage = ComputeTeamDamageProducedPerSecond(them);
        var ourDamage = ComputeTeamDamageProducedPerSecond(us);

        enemyArmor = SubtractDamageFromArmor(enemyArmor, ourDamage);

        DebugText =
            "Enemy armor: \n" + DictSortedAndToString(enemyArmor) + "\n" +
            "Enemy damage: \n" + DictSortedAndToString(enemyDamage);

        IEnumerable<float> damageScores = Map(options.Select(x => ComputeUnitDamageScore(x, enemyArmor)), 0f, 1f);
        IEnumerable<float> armorScores = Map(options.Select(x => ComputeUnitHealthScore(x, enemyDamage)), 0f, 1f);
        IEnumerable<float> costScores = Map(options.Select(x => (float)x.GetComponent<Unit>().GetCost(_commander)), 1f, 0f);
        IEnumerable<float> rangeScores = Map(options.Select(x => ComputeUnitRangeScore(x)), 0f, 1f);

        var scores = options.Zip(damageScores, (o, d) => new { Option = o, Damage = d })
            .Zip(armorScores, (od, a) => new { od.Option, od.Damage, Armor = a })
            .Zip(costScores, (oda, c) => new { oda.Option, oda.Damage, oda.Armor, Cost = c })
            .Zip(rangeScores, (odac, r) => new { odac.Option, odac.Damage, odac.Armor, odac.Cost, Range = r });
        
        foreach (var score in scores)
        {
            Unit unit = score.Option.GetComponent<Unit>();
            if (unit.GetWeapons().Any() && !SpecialistTags.Any(x => unit.Info.Tags.Contains(x)))
            {
                float total = DamageWeight + HealthWeight + CostWeight + RangeWeight;
                float avg = (
                    score.Damage * DamageWeight + 
                    score.Armor * HealthWeight + 
                    score.Cost * CostWeight +
                    score.Range * HealthWeight
                    ) / total;
                results.Add(score.Option, avg);
            }
            else
            {
                results.Add(score.Option, 0f); // Units without weapons can't really counter anything.
            }
        }

        return results;
    }

    public static string DictSortedAndToString<T>(Dictionary<T, float> dict)
    {
        var values = dict.Values.ToArray();
        var keys = dict.Keys.ToArray();
        StringBuilder builder = new StringBuilder();
        Array.Sort(values, keys);

        Array.Reverse(values);
        Array.Reverse(keys);

        for (int i = 0; i < values.Length; i++)
        {
            builder.AppendLine(keys[i] + ": " + values[i]);
        }
        return builder.ToString();
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        _commander = commander;
    }

    public override UnitWeightTableBase DeepCopy() => Instantiate(this);

    // Under the assumption that it becomes exponentially more dangerous the closer you are to the target,
    // It follows to reason that it should be logarithmically less dangerous the further you are. What should the base be? idk lol, I guess growth is the same for all bases.'
    // This may be completely wrong lol i'm not a math guy.
    private float ComputeUnitRangeScore(GameObject prefab)
    {
        float def = Mathf.Log(50, 2f); // 50 is conventionally the "default" range in this game.
        var controller = prefab.GetComponent<AttackerController>();
        if (controller != null)
        {
            return Mathf.Log(controller.HoldRange, 2f);
        }
        return def;
    }

    // Unit damage score indicates how much the units particular damage type(s) is needed at the moment.
    // High score indicates that the unit does high damage against enemy units which are not countered.
    // Score is computed from our current damage produced subtracted from enemy health produced, then multiplied by the weapons DPS against the enemy armor.
    // Scores for all weapons for each enemy armor type is summed. This should result in units whose effective DPS against uncountered units having higher scores.
    private float ComputeUnitDamageScore (GameObject prefab, Dictionary<DamageModifier, float> enemyArmor)
    {
        Unit unit = prefab.GetComponent<Unit>();
        float productionTime = GetUnitProductionTime(prefab);
        var weapons = unit.GetWeapons();
        float score = 0f;
        foreach (var weapon in weapons)
        {
            var type = weapon.Modifier;
            float dpsps = weapon.GetDPSOrOverride() / productionTime;
            if (type == null)
                Debug.Log(prefab, prefab);

            foreach (var ak in enemyArmor.Keys)
            {
                score += dpsps * DamageModifier.Combine(ak, type) * Mathf.Max(enemyArmor[ak], 0f);
            }
        }
        return score;
    }

    // Unit health score is computed based on the current DPS per second produced by the enemy forces.
    // Higher scores indicate that the particular unit is more resistent to what is produced by the enemy.
    // Computed from unit health multiplied by one minus enemy DPS effeciency against armor.
    // Scores for all healths for each enemy damage type is summed. This should result in armor that is less countered getting higher scores.
    private float ComputeUnitHealthScore (GameObject prefab, Dictionary<DamageModifier, float> enemyDamage)
    {
        float productionTime = GetUnitProductionTime(prefab);
        var healths = prefab.GetComponentsInChildren<Health>();
        float score = 0f;
        foreach (var health in healths)
        {
            var type = health.Modifier;
            float aps = health.MaxHealth / productionTime;
            foreach (var dk in enemyDamage.Keys)
            {
                score += aps * ((1f-DamageModifier.Combine(type, dk)) * enemyDamage[dk]);
            }
        }
        return score;
    }

    private Dictionary<DamageModifier, float> SubtractDamageFromArmor(Dictionary<DamageModifier, float> armor, Dictionary<DamageModifier, float> damage)
    {
        Dictionary<DamageModifier, float> result = new Dictionary<DamageModifier, float>(armor);
        foreach (var ak in armor.Keys)
        {
            foreach (var dk in damage.Keys)
            {
                float factor = DamageModifier.Combine(ak, dk);
                if (!result.ContainsKey(ak))
                    result.Add(ak, 0f);
                result[ak] -= factor * damage[dk];
            }
        }
        return result;
    }

    private Dictionary<DamageModifier, float> ComputeTeamDamageProducedPerSecond(IEnumerable<Commander> team)
    {
        Dictionary<DamageModifier, float> result = new Dictionary<DamageModifier, float>();
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
                var type = weapon.Modifier;
                var dpsps = weapon.GetDPSOrOverride() / productionTime;

                if (!result.ContainsKey(type))
                    result.Add(type, 0f);
                result[type] += dpsps;
            }
        }

        return result;
    }

    private Dictionary<DamageModifier, float> ComputeTeamArmorProducedPerSecond(IEnumerable<Commander> team)
    {
        Dictionary<DamageModifier, float> result = new Dictionary<DamageModifier, float>();
        var factories = team.SelectMany(x => x.AlivePlaced).Select(x => x.GetComponent<UnitFactory>()).Where(x => x != null);
        foreach (var factory in factories)
        {
            GameObject prefab = factory.UnitPrefab;
            Unit unit = prefab.GetComponent<Unit>();
            float productionTime = GetUnitProductionTime(prefab);
            var healths = prefab.GetComponentsInChildren<Health>();

            foreach (var health in healths)
            {
                var type = health.Modifier;
                var hps = health.MaxHealth / productionTime;
                if (type == null) {
                    Debug.Log(health.transform.root.name);
                }

                if (!result.ContainsKey(type))
                    result.Add(type, 0f);
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
}
