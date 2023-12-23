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

    public bool IsValid() => Collider && Collider.enabled && Collider.gameObject.activeSelf;

    public Vector3 GetCenter()
    {
        if (Collider == null)
        {
            return Vector3.zero;
        }
        return Collider.bounds.center;
    }

    public Vector3 GetSize()
    {
        if (Collider == null)
        {
            return Vector3.zero;
        }
        return Collider.bounds.size;
    }

    public GameObject GetGameObject()
    {
        if (Collider)
        {
            return Collider.gameObject;
        }
        return null;
    }
}
