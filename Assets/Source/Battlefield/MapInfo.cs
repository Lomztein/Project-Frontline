using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

[System.Serializable]
public class MapInfo
{
    public float Width => Bounds.size.x;
    public float Height => Bounds.size.y;

    public Bounds Bounds => GenerateBounds();

    public BattlefieldShape Shape;
    public SceneryGenerator SceneryGenerator;

    public IEnumerable<Vector3> GetPerimeterPolygon()
        => Shape.GetPerimeterPolygon(this);

    public bool Contains(Vector3 point)
        => GeometryXZ.IsInsidePolygon(GetPerimeterPolygon(), new Vector3(point.x, 0f, point.z));
    public float DistanceToEdge(Vector3 point)
        => GeometryXZ.DistanceFromPolygon(GetPerimeterPolygon(), new Vector3(point.x, 0f, point.z));

    public Vector2 Size => Bounds.size;

    private Bounds GenerateBounds()
    {
        Bounds bounds = new Bounds();
        foreach (var vec in GetPerimeterPolygon())
        {
            bounds.Encapsulate(vec);
        }
        return bounds;
    }
}
