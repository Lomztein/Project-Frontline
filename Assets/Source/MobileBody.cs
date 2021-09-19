using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public abstract class MobileBody : MonoBehaviour
{
    public float MaxSpeed;
    public Rigidbody Rigidbody;

    public Vector3 Velocity => transform.forward * CurrentSpeed;
    public abstract float CurrentSpeed { get; protected set; }

    protected virtual void Awake ()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.isKinematic = true;
        Rigidbody.useGravity = false; // I shouldn't have to do this but some Unity bug causes the rigidbody settings to not be saved on prefab.
    }

    protected void Move (Vector3 direction)
    {
        Rigidbody.MovePosition(transform.position + direction);
    }

    protected void Rotate (Quaternion rotation)
    {
        Rigidbody.MoveRotation(transform.rotation * rotation);
    }

    protected void Rotate(float x, float y, float z) => Rotate(Quaternion.Euler(x, y, z));
    protected void Rotate(Vector3 euler) => Rotate(Quaternion.Euler(euler));
}
