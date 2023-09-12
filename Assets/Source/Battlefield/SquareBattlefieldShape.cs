using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBattlefieldShape : IBattlefieldShape
{
    public string DisplayName => "Square";
    public int MaxTeams => 2;

    public IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;
        var lines = new SpawnLine[2];

        lines[0] = new SpawnLine(
            new Vector3(0f, 0f, halfWidth - 75),
            new Vector3(1f, 0f, 0f), 30f);

        lines[1] = new SpawnLine(
            new Vector3(0f, 0f, -halfWidth + 75),
            new Vector3(1f, 0f, 0f), 30f);

        return lines;
    }

    public IEnumerable<IWaypoint> GenerateWaypoints(MapInfo info)
    {
        float halfWidth = info.Width / 2f;
        var list = new List<Waypoint>
        {
            // Left side
            Waypoint.Create(new Vector3(0f, 0f, halfWidth), null, null),
            // Right side
            Waypoint.Create(new Vector3(0f, 0f, -halfWidth), null, null)
        };

        list[0].Next = list[1];
        list[1].Next = list[0];
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
