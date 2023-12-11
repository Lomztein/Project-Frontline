using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutators/Range Multiplier")]
public class RangeMultiplierMutator : Mutator
{
    public float RangeMultiplier;
    public float ProjectileSpeedMultiplier;
    public float WeaponAccuracyMultiplier;

    public override void Start()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander");
        foreach (var commanderObj in commanders)
        {
            var commander = commanderObj.GetComponent<Commander>();
            commander.OnUnitPlaced += Commander_OnUnitPlaced;
            commander.OnUnitSpawned += Commander_OnUnitSpawned;
        }
    }

    private void Commander_OnUnitSpawned(Commander arg1, UnitFactory arg2, Unit arg3)
    {
        Multiply(arg3);
    }

    private void Commander_OnUnitPlaced(Commander arg1, Unit arg2)
    {
        Multiply(arg2);
    }

    private void Multiply(Unit unit)
    {
        var ais = unit.GetComponentsInChildren<AIController>();
        foreach (var ai in ais)
        {
            ai.AcquireTargetRange *= RangeMultiplier;
            ai.LooseTargetRange *= RangeMultiplier;
            ai.AttackRange *= RangeMultiplier;
            if (ai is AttackerController atk) atk.HoldRange *= RangeMultiplier;
        }

        var weapons = unit.GetComponentsInChildren<Weapon>();
        foreach (var weapon in weapons)
        {
            weapon.Speed *= ProjectileSpeedMultiplier;
            weapon.Range *= RangeMultiplier;
            weapon.Inaccuracy /= WeaponAccuracyMultiplier;
        }

        var ats = unit.GetComponentsInChildren<ArtillaryTurret>();
        foreach (var at in ats)
        {
            at.ProjectileSpeed *= ProjectileSpeedMultiplier;
        }

        var infAts = unit.GetComponentsInChildren<ArtilleryTurretAdapter>();
        foreach (var inf in infAts)
        {
            inf.ProjectileSpeed *= ProjectileSpeedMultiplier;
        }
    }

    public override void Stop()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander");
        foreach (var commanderObj in commanders)
        {
            var commander = commanderObj.GetComponent<Commander>();
            commander.OnUnitPlaced -= Commander_OnUnitPlaced;
            commander.OnUnitSpawned -= Commander_OnUnitSpawned;
        }
    }
}
