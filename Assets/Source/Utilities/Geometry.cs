using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeometryXZ
{
    // Define Infinite (Using INT_MAX
    // caused overflow problems)
    private static int INF = 10000;

    // Given three collinear points p, q, r,
    // the function checks if point q lies
    // on line segment 'pr'
    public static bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) &&
            q.x >= Mathf.Min(p.x, r.x) &&
            q.z <= Mathf.Max(p.z, r.z) &&
            q.z >= Mathf.Min(p.z, r.z))
        {
            return true;
        }
        return false;
    }

    // To find orientation of ordered triplet (p, q, r).
    // The function returns following values
    // 0 --> p, q and r are collinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    public static int Orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        float val = (q.z - p.z) * (r.x - q.x) -
                (q.x - p.x) * (r.z - q.z);

        if ((int)val == 0)
        {
            return 0; // collinear
        }
        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }

    // The function that returns true if
    // line segment 'p1q1' and 'p2q2' intersect.
    public static bool DoIntersect(Vector3 p1, Vector3 q1,
                            Vector3 p2, Vector3 q2)
    {
        // Find the four orientations needed for
        // general and special cases
        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        // General case
        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        // Special Cases
        // p1, q1 and p2 are collinear and
        // p2 lies on segment p1q1
        if (o1 == 0 && OnSegment(p1, p2, q1))
        {
            return true;
        }

        // p1, q1 and p2 are collinear and
        // q2 lies on segment p1q1
        if (o2 == 0 && OnSegment(p1, q2, q1))
        {
            return true;
        }

        // p2, q2 and p1 are collinear and
        // p1 lies on segment p2q2
        if (o3 == 0 && OnSegment(p2, p1, q2))
        {
            return true;
        }

        // p2, q2 and q1 are collinear and
        // q1 lies on segment p2q2
        if (o4 == 0 && OnSegment(p2, q1, q2))
        {
            return true;
        }

        // Doesn't fall in any of the above cases
        return false;
    }

    // Returns true if the point p lies
    // inside the polygon[] with n vertices
    public static bool IsInsidePolygon(IEnumerable<Vector3> polygon, Vector3 p)
    {
        Vector3[] arr = polygon.ToArray();
        int n = arr.Length;
        // There must be at least 3 vertices in polygon[]
        if (n < 3)
        {
            return false;
        }

        // Create a point for line segment from p to infinite
        Vector3 extreme = new Vector3(INF, p.z, p.z);

        // Count intersections of the above line
        // with sides of polygon
        int count = 0, i = 0;
        do
        {
            int next = (i + 1) % n;

            // Check if the line segment from 'p' to
            // 'extreme' intersects with the line
            // segment from 'polygon[i]' to 'polygon[next]'
            if (DoIntersect(arr[i],
                            arr[next], p, extreme))
            {
                // If the point 'p' is collinear with line
                // segment 'i-next', then check if it lies
                // on segment. If it lies, return true, otherwise false
                if (Orientation(arr[i], p, arr[next]) == 0)
                {
                    return OnSegment(arr[i], p,
                                    arr[next]);
                }
                count++;
            }
            i = next;
        } while (i != 0);

        // Return true if count is odd, false otherwise
        return (count % 2 == 1); // Same as (count%2 == 1)
    }

    public static float DistanceFromPolygon(IEnumerable<Vector3> polygon, Vector3 x)
    {
        Vector3[] arr = polygon.ToArray();
        float minDist = float.MaxValue;
        for (int i = 0; i < arr.Length; i++)
        {
            var p1 = arr[i];
            var p2 = arr[(i + 1) & arr.Length - 1];

            var r = Vector3.Dot(p2 - p1, x - p1);
            r /= Vector3.SqrMagnitude(p2 - p1);

            float dist;
            if (r < 0)
            {
                dist = (x - p1).magnitude;
            }
            else if (r > 1)
            {
                dist = (p2 - x).magnitude;
            }
            else
            {
                dist = Mathf.Sqrt(Mathf.Pow((x - p1).magnitude, 2) - Mathf.Pow(r * (p2 - p1).magnitude, 2));
            }
            minDist = Mathf.Min(dist, minDist);
        }
        return minDist;
    }
}
