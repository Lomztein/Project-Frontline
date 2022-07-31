using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlefieldShape
{
    public IEnumerable<Vector3> GetPerimeterPolygon(BattlefieldInfo info);

    public IEnumerable<Waypoint> GenerateWaypoints(BattlefieldInfo info);
}
