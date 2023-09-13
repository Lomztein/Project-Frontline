using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New SquareBattlefieldShape", menuName = "Battlefield Shapes/Square")]
public class SquareBattlefieldShape : BattlefieldShape
{
    public override int Spawns => 2;

    public override IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;
        var lines = new SpawnLine[2];

        lines[0] = new SpawnLine(
            new Vector3(0f, 0f, halfWidth - 75), Quaternion.LookRotation(Vector3.right),
            new Vector3(1f, 0f, 0f), 30f);

        lines[1] = new SpawnLine(
            new Vector3(0f, 0f, -halfWidth + 75), Quaternion.LookRotation(Vector3.left),
            new Vector3(1f, 0f, 0f), 30f);

        return lines;
    }

    public override IEnumerable<NavigationNode> GenerateNodes(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        var list = new List<NavigationNode>
        {
            // Left side
            NavigationNode.Create(new Vector3(0f, 0f, halfWidth), null),
            // Right side
            NavigationNode.Create(new Vector3(0f, 0f, -halfWidth), null)
        };

        list[0].Neighbours = new NavigationNode[] { list[1] };
        list[1].Neighbours = new NavigationNode[] { list[0] };
        return list;
    }

    public override IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;

        yield return new Vector3(halfHeight, 0f, halfWidth);
        yield return new Vector3(halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, halfWidth);
    }
}
