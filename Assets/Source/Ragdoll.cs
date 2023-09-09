using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Ragdoll : MonoBehaviour
{
    private int _layer;
    private Rigidbody[] _rigidbodies;
    private Collider[] _colliders;
    private bool _cached;

    public Animator Animator;

    private void TryCache ()
    {
        if (!_cached)
        {
            _layer = LayerMask.NameToLayer("Ragdoll");
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
            _cached = true;
        }
    }

    public void OnEnable()
    {
        TryCache();
        if (Animator)
            Animator.enabled = false;
        SetRagdollEnabled(true);
    }

    public void OnDisable()
    {
        TryCache();
        if (Animator)
            Animator.enabled = true;
        SetRagdollEnabled(false);   
    }

    public void AddForce (Vector3 position, Vector3 force)
    {
        GetClosestRigidbody(position).AddForceAtPosition(force, position, ForceMode.Impulse);
    }

    private Rigidbody GetClosestRigidbody (Vector3 position)
    {
        TryCache();
        Rigidbody closest = null;
        float dist = float.MaxValue;
        foreach (Rigidbody rb in _rigidbodies)
        {
            float d = Vector3.SqrMagnitude(rb.transform.position - position);
            if (dist > d)
            {
                dist = d;
                closest = rb;
            }
        }
        return closest;
    }

    private void SetRagdollEnabled (bool value)
    {
        foreach (Rigidbody rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = !value;
            rigidbody.detectCollisions = value;
        }
        foreach (Collider collider in _colliders)
        {
            collider.enabled = value;
            collider.gameObject.layer = _layer;
        }
    }
}
