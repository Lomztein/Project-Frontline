using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util;

[CreateAssetMenu(fileName = "New EllipsoidBattlefieldShape", menuName = "Battlefield Shapes/Ellipsoid")]
public class EllipsoidBattlefieldShape : BattlefieldShape
{
    [SerializeField] private int _spawns;
    public override int Spawns => _spawns;

    public float SpawnDepth;
    public int Verts;

    public override IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        float width = info.Width / 2f;
        float height = info.Height / 2f;
        int spawns = Spawns;
        for (int i = 0; i < spawns; i++)
        {
            float rads = (i / (float)spawns) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * width;  
            float x = Mathf.Sin(rads) * height;

            var dir = new Vector3(x, 0f, z).normalized * -1f;
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            yield return new SpawnLine(new Vector3(x, 0f, z) + dir * SpawnDepth, Quaternion.LookRotation(dir, Vector3.up), left, 30);
        }
    }

    public override IEnumerable<NavigationNode> GenerateNodes(MapInfo info)
    {
        float width = info.Width / 2f;
        float height = info.Height / 2f;

        var spawns = Spawns;
        NavigationNode center = NavigationNode.Create(Vector3.zero, new NavigationNode[spawns]);

        for (int i = 0; i < spawns; i++)
        {
            float rads = (i / (float)spawns) * Mathf.PI * 2f;
            float z = Mathf.Cos(rads) * width;
            float x = Mathf.Sin(rads) * height;

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
        float width = info.Width / 2f;
        float height = info.Height / 2f;
        int verts = Verts;
        for (int i = 0; i < verts; i++)
        {
            float rads = -((i / (float)verts) * Mathf.PI * 2f);
            float offset = Mathf.PI / (float)verts;
            float z = Mathf.Cos(rads + offset) * width;
            float x = Mathf.Sin(rads + offset) * height;
            yield return new Vector3(x, 0f, z);
        }
    }
}
