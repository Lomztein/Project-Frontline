using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool CanUpgrade(GameObject unit) => CheckFilter(unit);

    private bool CheckFilter(GameObject unit)
        => Filter == null ? true : Filter.Check(unit.gameObject);

    private void Commander_OnUnitSpawned(Commander arg1, UnitFactory arg2, Unit arg3)
    {
        float rand = Random.Range(0f, 1f);
        float chance = ComputeChance();

        if (rand < chance && CheckFilter(arg3.gameObject))
        {
            ApplyUpgrade(arg3);
        }
    }

    protected abstract void ApplyUpgrade(Unit target);

    public override bool CanPurchase(Unit unit, Commander commander)
        => commander.AlivePlaced.Select(x => commander.GetProducedUnitIfFactory(x.gameObject)).Any(x => CanUpgrade(x));

    public override string GetDescription(Unit unit, Commander commander)
    {
        if (!commander.CanPurchase(unit.gameObject))
        {
            return "Unavailable: No affected units on field.";
        }
        return null;
    }
}
