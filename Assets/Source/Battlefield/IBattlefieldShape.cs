using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefieldShape
{
    public IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info);

    public IEnumerable<Waypoint> GenerateWaypoints(MapInfo info);

    public IEnumerable<SpawnLine> GenerateSpawnLines(MapInfo info);
}
