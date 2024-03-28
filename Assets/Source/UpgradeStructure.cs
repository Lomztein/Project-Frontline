using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: If the "first" structure is destroyed, the upgrade event is removed even if there are other structures of the same type.
// Change "first" system to a "master" system where every structure subscribes to the event, but only the "master" actually executes the upgrade.
// The master structure is simply the first in the list, updated whenever a structure is destroyed.

public abstract class UpgradeStructure : MonoBehaviour, ICommanderComponent, IUnitPurchasePredicate
{
    public string UpgradeIdentifier;
    protected Commander _commander;

    public IUnitPurchasePredicate.AnyAll Behaviour => IUnitPurchasePredicate.AnyAll.Any;

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }

    private void Start()
    {
        UpgradeStructure first = GetFirst();
        if (this == first)
        {
            ApplyInitial();
        }
        else
        {
            ApplyStack(first);
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && _commander)
        {
            UpgradeStructure first = GetFirst();
            if (this == first || first == null)
            {
                RemoveInitial();
            }
            else
            {
                RemoveStack(first);
            }
        }
    }

    protected UpgradeStructure GetFirst()
    {
        foreach (var unit in _commander.GetPlacedUnits())
        {
            var upgStructures = unit.GetComponents<UpgradeStructure>();
            foreach (var upgStructure in upgStructures)
            {
                if (upgStructure && upgStructure.UpgradeIdentifier == UpgradeIdentifier)
                {
                    return upgStructure;
                }
            }
        }
        return null;
    }

    protected abstract void ApplyInitial();

    protected abstract void ApplyStack(UpgradeStructure initial);

    protected abstract void RemoveInitial();

    protected abstract void RemoveStack(UpgradeStructure initial);

    public abstract bool CanPurchase(Unit unit, Commander commander);
    public abstract string GetDescription(Unit unit, Commander commander);
}
