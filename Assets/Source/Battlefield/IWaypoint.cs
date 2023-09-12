using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWaypoint
{
    public Vector3 Position { get; }

    public bool IsValid();

    public IWaypoint GetNext();
    public IWaypoint GetPrev();
}