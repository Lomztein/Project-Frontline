using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Faction")]
public class Faction : ScriptableObject
{
    public const string FACTIONS_RESOURCE_BASE_PATH = "Factions/";
    public const string DEFAULT_RESOURCE_PATH = "ModernMilitary";
    public const string UNIT_RESOURCE_BASE_PATH = "Units/";

    public string Name;
    [TextArea]
    public string Description;
    public string UnitsResourcePath;

    public GameObject HeadquartersPrefab;

    public IEnumerable<GameObject> LoadUnits()
        => Resources.LoadAll<GameObject>(UNIT_RESOURCE_BASE_PATH + UnitsResourcePath);

    public static IEnumerable<Faction> LoadFactions()
        => Resources.LoadAll<Faction>(FACTIONS_RESOURCE_BASE_PATH);

    public static Faction LoadDefault()
        => Resources.Load<Faction>(FACTIONS_RESOURCE_BASE_PATH + DEFAULT_RESOURCE_PATH);
}
