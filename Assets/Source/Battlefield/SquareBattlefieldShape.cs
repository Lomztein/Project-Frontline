using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareBattlefieldShape : IBattlefieldShape
{
    public IEnumerable<Waypoint> GenerateWaypoints(BattlefieldInfo info)
    {
        float halfWidth = info.Width / 2f;

        // Left side
        yield return Waypoint.Create(new Vector3(halfWidth, 0f), Vector3.left, Vector3.right);
        // Right side
        yield return Waypoint.Create(new Vector3(-halfWidth, 0f), Vector3.right, Vector3.left);
    }

    public IEnumerable<Vector3> GetPerimeterPolygon(BattlefieldInfo info)
    {
        float halfWidth = info.Width / 2f;
        float halfHeight = info.Height / 2f;

        yield return new Vector3(halfWidth, 0f, halfHeight);
        yield return new Vector3(halfWidth, 0f, -halfHeight);
        yield return new Vector3(-halfWidth, 0f, -halfHeight);
        yield return new Vector3(-halfWidth, 0f, halfHeight);
    }
}
