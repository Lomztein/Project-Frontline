using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefieldShape
{
    public string DisplayName { get; }
    public int MaxTeams { get; }

    public IEnumerable<Vector3> GetPerimeterPolygon(MapInfo info);

    public IEnumerable<IWaypoint> GenerateWaypoints(MapInfo info);

    public IEnumerable<ISpawnVolume> GenerateSpawnVolumes(MapInfo info);
}
