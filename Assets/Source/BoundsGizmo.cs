using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class BoundsGizmo : MonoBehaviour
{
    public GameObject Object;

    private void OnDrawGizmos()
    {
        if (Object)
        {
            Bounds bounds = UnityUtils.ComputeMinimallyBoundingBox(Object);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
