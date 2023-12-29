using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Vector3 From;
    public Vector3 To;

    public Line(Vector3 from, Vector3 to)
    {
        From = from;
        To = to;
    }

    public Vector3 ClosestPositionOnLine(Vector3 position)
        => GeometryXZ.NearestPointOnLine(From, (To - From), position);
}