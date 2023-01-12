using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Faction", menuName = "Faction")]
public class Faction : ScriptableObject
{
    public const string FACTIONS_RESOURCE_BASE_PATH = "Factions/";
    public const string UNIVERSAL_UNIT_RESOURCE_PATH = "Units/Universal";
    private GameObject[] _unitCache;

    public string Name;
    [TextArea]
    public string Description;
    public string[] UnitsResourcePaths;
    public Texture2D FactionPalette;

    public GameObject HeadquartersPrefab;

    public GameObject[] LoadUnits()
    {
        if (_unitCache == null)
        {
            var list = new List<GameObject>();
            list.AddRange(Resources.LoadAll<GameObject>(UNIVERSAL_UNIT_RESOURCE_PATH));
            foreach (string path in UnitsResourcePaths)
            {
                list.AddRange(Resources.LoadAll<GameObject>(path));
            }
            _unitCache = list.Distinct().ToArray();
        }
        return _unitCache;
    }

    public void OnValidate()
    {
        _unitCache = null;
    }

    public static IEnumerable<Faction> LoadFactions()
        => Resources.LoadAll<Faction>(FACTIONS_RESOURCE_BASE_PATH);
}
