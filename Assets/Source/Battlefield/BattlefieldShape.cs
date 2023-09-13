using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattlefieldShape : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private string _description;
    public string Description => _description;
    public abstract int Spawns { get; }

    public abstract IEnumerable<NavigationNode> GenerateNodes(MapInfo info);
    public abstract IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info);
    public abstract IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info);
}
