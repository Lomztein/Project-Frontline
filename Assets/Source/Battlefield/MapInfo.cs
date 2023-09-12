using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[System.Serializable]
public class MapInfo
{
    public float Width;
    public float Height;

    [SerializeReference, SR]
    public IBattlefieldShape Shape;
    public SceneryGenerator SceneryGenerator;

    public IEnumerable<Vector3> GetPerimeterPolygon()
        => Shape.GetPerimeterPolygon(this);

    public bool Contains(Vector3 point)
        => GeometryXZ.IsInsidePolygon(GetPerimeterPolygon(), new Vector3(point.x, 0f, point.z));
    public float DistanceToEdge(Vector3 point)
        => GeometryXZ.DistanceFromPolygon(GetPerimeterPolygon(), new Vector3(point.x, 0f, point.z));

    public Vector2 Size => new Vector2(Height, Width);
}
