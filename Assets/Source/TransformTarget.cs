using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTarget : ITarget
{
    private Transform _transform;

    public TransformTarget (Transform transform)
    {
        _transform = transform;
    }

    public bool IsValid() => _transform;

    public Vector3 GetPosition()
    {
        if (_transform == null)
        {
            return Vector3.zero;
        }
        return _transform.position;
    }
}
