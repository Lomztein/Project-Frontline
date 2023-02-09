using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Composite Delta Weight Table", menuName = "Unit Weight Tables/Composite Delta")]
public class CompositionDeltaWeightTable : UnitWeightTableBase
{
    public float CountStructures;
    public float FallbackProductionTime;
    public string[] SpecialistTags;

    private Commander _commander;
    private static Dictionary<DamageMatrix.Damage, Dictionary<DamageMatrix.Armor, float>> _damageMappingCopy;

    public override Dictionary<GameObject, float> GetWeights(IEnumerable<GameObject> options)
    {
        if (_damageMappingCopy == null) // Cache a copy of the damage mapping.
            _damageMappingCopy = DamageMatrix.CopyMapping();

        var enemyDamage = Enum.GetValues(typeof(DamageMatrix.Damage)).Cast<DamageMatrix.Damage>().ToDictionary(x => x, y => 0f);
        var enemyArmorMinusAlliedDamage = Enum.GetValues(typeof(DamageMatrix.Armor)).Cast<DamageMatrix.Armor>().ToDictionary(x => x, y => 0f);

        var enemyPlaced = Team.GetOtherTeams(_commander.TeamInfo).SelectMany(x => x.GetCommanders()).SelectMany(x => x.GetPlacedUnits());
        var alliedPlaced = Team.GetTeam(_commander.TeamInfo).GetCommanders().SelectMany(x => x.GetPlacedUnits());

        foreach (var placed in enemyPlaced)
        {
            // Aggregate enemy damage - lowest damage type is the one we want to build armor against.
            (Unit unit, float productionTime) = GetUnitProductionInfo(placed);

            // Disregard all but factories.
            if (unit.CompareTag("StructureUnit")) continue;

            foreach (IWeapon weapon in unit.GetWeapons())
            {
                enemyDamage[weapon.DamageType] += weapon.GetDPSOrOverride() / productionTime;
            }

            // Aggregate enemy armor.
            Health health = unit.GetComponent<Health>();
            enemyArmorMinusAlliedDamage[health.ArmorType] += health.MaxHealth / productionTime;
        }
        
        // Subtract our damage from enemy armor - highest remaining armor type is the one we need to counter.
        foreach (var placed in alliedPlaced)
        {
            (Unit unit, float productionTime) = GetUnitProductionInfo(placed);

            // Disregard all but factories.
            if (unit.CompareTag("StructureUnit")) continue;

            foreach (IWeapon weapon in unit.GetWeapons())
            {
                foreach (var value in Enum.GetValues(typeof(DamageMatrix.Armor)))
                {
                    var armorType = (DamageMatrix.Armor)value;
                    float dpsVsArmor = weapon.GetDPSOrOverride() * DamageMatrix.GetDamageFactor(weapon.DamageType, armorType);
                    enemyArmorMinusAlliedDamage[armorType] -= dpsVsArmor / productionTime;
                }
            }
        }

        // Compute unit weights for each unit and normalize.
        var weights = new Dictionary<GameObject, float>();
        float highestScore = 0.1f;

        // Compute weights
        foreach (var option in options)
        {
            float score = ComputeUnitScore(option, enemyDamage, enemyArmorMinusAlliedDamage);
            if (score > highestScore)
            {
                highestScore = score;
            }
            weights.Add(option, score);
        }

        // Normalize
        foreach (var option in options)
        {
            weights[option] /= highestScore;
        }

        return weights;
    }

    private float ComputeUnitScore (GameObject go, Dictionary<DamageMatrix.Damage, float> enemyDamage, Dictionary<DamageMatrix.Armor, float> enemyArmorMinusAlliedDamage)
    {
        float armorScore = 0f;
        float damageScore = 0f;
        (Unit unit, float productionTime) = GetUnitProductionInfo(go.GetComponent<Unit>());
        var goUnit = go.GetComponent<Unit>(); // bit wierd I know


        Health[] healths = go.GetComponentsInChildren<Health>();
        foreach (var pair in enemyDamage)
        {
            foreach (Health health in healths)
            {
                armorScore -= DamageMatrix.GetDamageFactor(pair.Key, health.ArmorType) * pair.Value / (health.MaxHealth / 2000f) / productionTime;
            }
        }

        var weapons = goUnit.GetWeapons();
        foreach (var pair in enemyArmorMinusAlliedDamage)
        {
            foreach (var weapon in weapons)
            {
                damageScore += weapon.GetDPSOrOverride() * DamageMatrix.GetDamageFactor(weapon.DamageType, pair.Key) * Mathf.Max(0f, pair.Value) / productionTime;
            }
        }
        // Specialists usually do a lot more damage than generalists, and as so half their damage score.
        if (SpecialistTags.Any(x => goUnit.Info.Tags.Contains(x))) damageScore /= 2f;

        return Mathf.Max (float.Epsilon, armorScore + damageScore);
    }

    private (Unit unit, float productionTime) GetUnitProductionInfo (Unit potentialFactory)
    {
        Unit unit = potentialFactory;
        float productionTime = FallbackProductionTime;

        UnitFactory factory = potentialFactory.GetComponent<UnitFactory>();
        if (factory)
        {
            unit = factory.UnitPrefab.GetComponent<Unit>();
            productionTime = unit.GetComponent<ProductionInfo>().ProductionTime;
        }

        ProductionInfo info = potentialFactory.GetComponent<ProductionInfo>();
        if (info)
        {
            productionTime = info.ProductionTime;
        }

        return (unit, productionTime);
    }

    public override void Initialize(Commander commander, IEnumerable<GameObject> availableUnits)
    {
        _commander = commander;
    }

    public override UnitWeightTableBase DeepCopy() => Instantiate(this);
}
