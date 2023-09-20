using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "New EllipsoidBattlefieldShape", menuName = "Battlefield Shapes/Ellipsoid")]
public class EllipsoidBattlefieldShape : BattlefieldShape
{
    [SerializeField] private int _spawns;
    public override int Spawns => _spawns;

    public float Radius;
    public float SpawnDepth;
    private int Verts => Spawns * 4;

    public RangeProperty SpawnsProperty = new("Spawns", "The amount of spawn points, spread evenly.", 2, 16, true);
    public RangeProperty RadiusProperty = new("Radius", "The radius of the map.", 100, 500, true);
    public RangeProperty SpawnDepthProperty = new("Spawn Depth", "How far from the edge do commanders spawn.", 10, 50, true);

    public override IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        int spawns = Spawns;
        for (int i = 0; i < spawns; i++)
        {
            float rads = (i / (float)spawns) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * Radius / 2f;  
            float x = Mathf.Sin(rads) * Radius / 2f;

            var dir = new Vector3(x, 0f, z).normalized * -1f;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            yield return new SpawnLine(new Vector3(x, 0f, z) + dir * SpawnDepth, Quaternion.LookRotation(dir, Vector3.up), left, 30);
        }
    }

    public override IEnumerable<NavigationNode> GenerateNodes(MapInfo info)
    {
        var spawns = Spawns;
        NavigationNode center = NavigationNode.Create(Vector3.zero, new NavigationNode[spawns]);

        for (int i = 0; i < spawns; i++)
        {
            float rads = (i / (float)spawns) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * Radius / 2f;
            float x = Mathf.Sin(rads) * Radius / 2f;

            var dir = new Vector3(x, 0f, z).normalized * -1f;
            var pos = new Vector3(x, 0f, z);

            NavigationNode newNode = NavigationNode.Create(pos + dir * SpawnDepth, new NavigationNode[] {center});
            center.Neighbours[i] = newNode;

            yield return newNode;
        }
        yield return center;
    }

    public override IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info)
    {
        int verts = Verts;
        for (int i = 0; i < verts; i++)
        {
            float rads = -((i / (float)verts) * Mathf.PI * 2f);
            float offset = Mathf.PI / (float)verts;
            float z = Mathf.Cos(rads + offset) * Radius / 2f;
            float x = Mathf.Sin(rads + offset) * Radius / 2f;
            yield return new Vector3(x, 0f, z);
        }
    }

    public override IEnumerable<IProperty> GetProperties()
    {
        yield return RadiusProperty;
        yield return SpawnsProperty;
        yield return SpawnDepthProperty;
    }

    public override bool SetProperty(IProperty property, object value)
    {
        if (property == RadiusProperty) Radius = (float)value;
        if (property == SpawnsProperty) _spawns = Mathf.RoundToInt((float)value);
        if (property == SpawnDepthProperty) SpawnDepth = (float)value;
        return true;
    }

    public override object GetProperty(IProperty property)
    {
        if (property == RadiusProperty) return Radius;
        if (property == SpawnsProperty) return Spawns;
        if (property == SpawnDepthProperty) return SpawnDepth;
        return null;
    }
}
