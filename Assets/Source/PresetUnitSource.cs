using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetUnitSource : UnitSource
{
    public GameObject[] AvailableUnits;

    public override GameObject[] GetAvailableUnitPrefabs(Faction faction)
    {
        return AvailableUnits;
    }
}
