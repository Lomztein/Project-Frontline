using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class FollowerCamera : MonoBehaviour
{
    public Transform FollowObject;
    public Vector3 BaseObjectSize;

    public Vector3 PositionOffset;
    public float AngleOffset = 15;

    public float MouseSensitivity = 10f;
    public float PositionLerpSpeed;

    private Vector3 _orbitLocalRotation = new Vector3(0f, 0f, 0f);

    private Vector3 _size;
    private Vector3 _colCenter;
    private Vector3 _center;

    private Collider _collider;

    virtual protected void FixedUpdate()
    {
        if (FollowObject)
        {
            Quaternion rot = Quaternion.Euler(_orbitLocalRotation);

            Vector3 rotated = rot * GetBoundsOffset(PositionOffset);
            Vector3 position = _center + rotated;

            float mult = BaseObjectSize.z > 0.01f ? _size.z / BaseObjectSize.z : 0f;

            transform.position = position;
            transform.rotation = rot * Quaternion.Euler(new Vector3(AngleOffset / mult, 0f, 0f));

            _center = Vector3.Lerp(_center, _collider.transform.position + _colCenter, PositionLerpSpeed * Time.fixedDeltaTime);
        }
    }

    private void Update()
    {
        Vector3 input = new Vector3 (-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        _orbitLocalRotation += input * MouseSensitivity * Time.unscaledDeltaTime;

        _orbitLocalRotation = new Vector3(
            Mathf.Clamp(_orbitLocalRotation.x, -85f - AngleOffset, 85f - AngleOffset),
            _orbitLocalRotation.y,
            _orbitLocalRotation.z);
    }

    private Vector3 GetBoundsOffset(Vector3 offset)
    {
        float xFactor = BaseObjectSize.x > 0.01f ? _size.x / BaseObjectSize.x : 0f;
        float yFactor = BaseObjectSize.y > 0.01f ? _size.y / BaseObjectSize.y : 0f;
        float zFactor = BaseObjectSize.z > 0.01f ? _size.z / BaseObjectSize.z : 0f;
        return new Vector3(offset.x * xFactor, offset.y * yFactor, offset.z * zFactor);
    }

    public void Follow(Transform obj)
    {
        FollowObject = obj;
        SetMouseStatus(false);
        
        _collider = obj.GetComponentInChildren<Collider>();
        if (_collider is BoxCollider box)
        {
            _size = box.size;
            _colCenter = box.center;
        }
        if (_collider is CapsuleCollider capsule)
        {
            _size = new Vector3(capsule.radius, capsule.height, capsule.radius);
            _colCenter = capsule.center;
        }
        if (_collider is SphereCollider sphere)
        {
            _size = new Vector3(sphere.radius, sphere.radius, sphere.radius);
            _colCenter = sphere.center;
        }
    }

    public void StopFollow ()
    {
        FollowObject = null;
        SetMouseStatus(true);
    }

    public void SetMouseStatus (bool enabled)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    protected virtual void OnDrawGizmos()
    {
        if (FollowObject)
        {
            Quaternion rot = Quaternion.Euler(_orbitLocalRotation);

            Vector3 rotated = rot * GetBoundsOffset(PositionOffset);
            Vector3 position = _colCenter + rotated;

            Gizmos.DrawSphere(position, 0.5f);
        }
    }
}
