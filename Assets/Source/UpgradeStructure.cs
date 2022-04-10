using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeStructure : MonoBehaviour, ICommanderComponent
{
    public string UpgradeIdentifier;
    protected Commander _commander;

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }

    // Start is called before the first frame update
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
            UpgradeStructure upgStructure = unit.GetComponent<UpgradeStructure>();
            if (upgStructure && upgStructure.UpgradeIdentifier == UpgradeIdentifier)
            {
                return upgStructure;
            }
        }
        return null;
    }

    protected abstract void ApplyInitial();

    protected abstract void ApplyStack(UpgradeStructure initial);

    protected abstract void RemoveInitial();

    protected abstract void RemoveStack(UpgradeStructure initial);

}
