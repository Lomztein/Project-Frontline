using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesUnitSource : UnitSource
{
    private const string UNIT_PATH = "Units";
    private GameObject[] _cache;

    public override GameObject[] GetAvailableUnitPrefabs()
    {
        if (_cache == null)
        {
            _cache = Resources.LoadAll<GameObject>(UNIT_PATH);
        }
        return _cache;
    }
}
