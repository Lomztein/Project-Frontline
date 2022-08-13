using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommanderUnitSource : UnitSource
{
    private Commander _commander;

    private void Awake()
    {
        _commander = GetComponent<Commander>();
    }

    public override GameObject[] GetAvailableUnitPrefabs(Faction faction)
    {
        var units = faction.LoadUnits();
        return units.Where(x => _commander.IsUnitAvailable(x)).ToArray();
    }
}
