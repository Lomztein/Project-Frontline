using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct OffcenterBounds
{
    public Vector3 Center;
    public Vector3 UpperExtends;
    public Vector3 LowerExtends;
    public Vector3 Size => UpperExtends - LowerExtends;

    public OffcenterBounds(Vector3 center, Vector3 upperExtends, Vector3 lowerExtends)
    {
        Center = center;
        UpperExtends = upperExtends;
        LowerExtends = lowerExtends;
    }

    public OffcenterBounds(Bounds bounds, Vector3 center)
    {
        Vector3 diff = center - bounds.center;
        Center = center;
        UpperExtends = bounds.extents + diff;
        LowerExtends = bounds.extents - diff;
    }

    public void Encapsulate(Vector3 point)
    {
        // I don't know if all of these operations are neccesary, but I don't really care to worry about it more than neccesary.
        if (point.x < LowerExtends.x) LowerExtends.x = point.x;
        if (point.x > UpperExtends.x) UpperExtends.x = point.x;
        if (point.y < LowerExtends.y) LowerExtends.y = point.y;
        if (point.y > UpperExtends.y) UpperExtends.y = point.y;
        if (point.z < LowerExtends.z) LowerExtends.z = point.z;
        if (point.z > UpperExtends.z) UpperExtends.z = point.z;
    }

    public void Encapsulate(Bounds bounds)
    {
        Encapsulate(bounds.center + bounds.extents);
        Encapsulate(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z));
        Encapsulate(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z));
    }

    public Bounds ToBounds()
    {
        Vector3 center = Center + (LowerExtends + (UpperExtends - LowerExtends) / 2f);
        return new Bounds(center, Size);
    }
}