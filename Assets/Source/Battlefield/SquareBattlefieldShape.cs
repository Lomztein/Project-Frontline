using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBattlefieldShape : IBattlefieldShape
{
    public IEnumerable<SpawnLine> GenerateSpawnLines(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;
        var lines = new SpawnLine[2];

        lines[0] = new SpawnLine(
            new Vector3(halfHeight - 75, 0f, halfWidth - 75),
            new Vector3(-halfHeight + 75, 0f, halfWidth - 75));

        lines[1] = new SpawnLine(
            new Vector3(halfHeight - 75, 0f, -halfWidth + 75),
            new Vector3(-halfHeight + 75, 0f, -halfWidth + 75));

        return lines;
    }

    public IEnumerable<Waypoint> GenerateWaypoints(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        var list = new List<Waypoint>
        {
            // Left side
            Waypoint.Create(new Vector3(0f, 0f, halfWidth), Vector3.forward, Vector3.back),
            // Right side
            Waypoint.Create(new Vector3(0f, 0f, -halfWidth), Vector3.back, Vector3.forward)
        };

        return list;
    }

    public IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;

        yield return new Vector3(halfHeight, 0f, halfWidth);
        yield return new Vector3(halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, -halfWidth);
        yield return new Vector3(-halfHeight, 0f, halfWidth);
    }
}
