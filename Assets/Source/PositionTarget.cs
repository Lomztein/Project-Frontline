using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTarget : ITarget
{
    private Vector3 _position;

    public PositionTarget (Vector3 position)
    {
        _position = position;
    }

    public bool IsValid() => true;

    public Vector3 GetCenter()
    {
        return _position;
    }

    public Vector3 GetSize()
    {
        return Vector3.one;
    }
}
