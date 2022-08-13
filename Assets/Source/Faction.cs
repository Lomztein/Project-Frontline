using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Faction")]
public class Faction : ScriptableObject
{
    public const string FACTIONS_RESOURCE_BASE_PATH = "Factions/";
    public const string DEFAULT_RESOURCE_PATH = "ModernMilitary";
    public const string UNIT_RESOURCE_BASE_PATH = "Units/";
    private GameObject[] _unitCache;

    public string Name;
    [TextArea]
    public string Description;
    public string UnitsResourcePath;

    public GameObject HeadquartersPrefab;

    public GameObject[] LoadUnits()
    {
        if (_unitCache == null)
        {
            _unitCache = Resources.LoadAll<GameObject>(UNIT_RESOURCE_BASE_PATH + UnitsResourcePath);
        }
        return _unitCache;
    }

    public static IEnumerable<Faction> LoadFactions()
        => Resources.LoadAll<Faction>(FACTIONS_RESOURCE_BASE_PATH);

    public static Faction LoadDefault()
        => Resources.Load<Faction>(FACTIONS_RESOURCE_BASE_PATH + DEFAULT_RESOURCE_PATH);

}
