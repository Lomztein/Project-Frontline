using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileBody : MonoBehaviour
{
    public float MaxSpeed;

    public Vector3 Velocity => transform.forward * CurrentSpeed;
    public abstract float CurrentSpeed { get; protected set; }
}
