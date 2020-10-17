using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTarget : ITarget
{
    public Collider Collider;

    public ColliderTarget (GameObject go)
    {
        if (go)
        {
            Collider = go.GetComponentInChildren<Collider>();
        }
    }

    public ColliderTarget (Collider col)
    {
        Collider = col;
    }

    public bool IsValid() => Collider;

    public Vector3 GetPosition()
    {
        if (Collider == null)
        {
            return Vector3.zero;
        }
        return Collider.bounds.center;
    }
}
