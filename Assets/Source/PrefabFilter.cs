using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Prefab Filter", menuName = "GameObject Filter/Prefab Filter")]
public class PrefabFilter : GameObjectFilter
{
    public GameObject[] AllowedPrefabs;

    public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objects)
        => objects.Where(x => AllowedPrefabs.Contains(x));
}
