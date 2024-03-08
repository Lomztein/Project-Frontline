using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineSegment
{
    public Line[] Lines;

    public LineSegment(Line[] lines)
    {
        Lines = lines;
    }

    public float GetContinuousIndexOfPosition(Vector3 position)
    {
        Line line = GeometryXZ.NearestLineToPoint(Lines, position);
        int index = Array.IndexOf(Lines, line);
        float t = GeometryXZ.InverseLerp(line.From, line.To, position);
        return index + t;
    }

    public Vector3 GetNearestPointOnLines(Vector3 position)
        => GeometryXZ.NearestPointOnLines(Lines, position);

    public Vector3 GetPosition(float continousIndex)
    {
        int lineIndex = Mathf.Clamp(Mathf.FloorToInt(continousIndex), 0, Lines.Length - 1);
        continousIndex -= lineIndex;
        Line line = Lines[lineIndex];

        return Vector3.Lerp(line.From, line.To, Mathf.Clamp01(continousIndex));
    }

    public static LineSegment CreateFrom(IEnumerable<Vector3> points)
    {
        if (points.Count() <= 1)
        {
            throw new InvalidOperationException("There must be at least two points to form a line.");
        }

        Vector3[] pointsArray = points.ToArray();

        // Transform into lines.
        Line[] lines = new Line[pointsArray.Length - 1];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = new Line(pointsArray[i], pointsArray[i + 1]);
        }

        return new LineSegment(lines);
    }
}
