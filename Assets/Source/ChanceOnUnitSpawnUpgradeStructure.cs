using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChanceOnUnitSpawnUpgradeStructure : UpgradeStructure
{
    public float BaseChance;
    public float ChancePerStack;
    private float _currentStack = 0f; // Should only tracked by the inital structure.
    public GameObjectFilter Filter;

    public float ComputeChance()
        => 1 - ((1 - BaseChance) * 1 / (_currentStack + 1));

    protected override void ApplyInitial()
    {
        _commander.OnUnitSpawned += Commander_OnUnitSpawned;
    }

    protected override void ApplyStack(UpgradeStructure initial)
    {
        (initial as ChanceOnUnitSpawnUpgradeStructure)._currentStack += ChancePerStack;
    }

    protected override void RemoveInitial()
    {
        _commander.OnUnitSpawned -= Commander_OnUnitSpawned;
    }

    protected override void RemoveStack(UpgradeStructure initial)
    {
        (initial as ChanceOnUnitSpawnUpgradeStructure)._currentStack -= ChancePerStack;
    }

    private bool CheckFilter(Unit unit)
        => Filter == null ? true : Filter.Check(unit.gameObject);

    private void Commander_OnUnitSpawned(Commander arg1, UnitFactory arg2, Unit arg3)
    {
        if (Random.Range(0f, 1f) < ComputeChance() && CheckFilter(arg3))
        {
            ApplyUpgrade(arg3);
        }
    }

    protected abstract void ApplyUpgrade(Unit target);
}
