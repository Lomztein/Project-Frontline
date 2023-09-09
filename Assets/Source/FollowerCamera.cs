using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class FollowerCamera : MonoBehaviour
{
    public Transform FollowObject;

    public float AngleToObject = 20;
    public float DistanceMultiplier = 2f;
    public float SizeMin = 4f;

    public float MouseSensitivity = 10f;
    public float PositionLerpSpeed;

    private Vector3 _orbitLocalRotation = new Vector3(0f, 0f, 0f);

    private Vector3 _size;
    private Vector3 _colCenter;
    private Vector3 _center;
    private Vector3 _targetPosition;

    private Collider _collider;

    virtual protected void FixedUpdate()
    {
        if (FollowObject)
        {
            Quaternion rot = Quaternion.Euler(_orbitLocalRotation);

            _targetPosition = _center + rot * ComputeLocalOffset();
            transform.position = _targetPosition;
            transform.rotation = rot;

            _center = Vector3.Lerp(_center, _collider.transform.position + _collider.transform.rotation * _colCenter, PositionLerpSpeed * Time.fixedDeltaTime);
        }
    }

    private Vector3 ComputeLocalOffset()
    {
        float x = 0f;
        float y = Mathf.Sin(AngleToObject * Mathf.Deg2Rad);
        float z = Mathf.Cos(AngleToObject * Mathf.Deg2Rad);

        float sizeMax = Mathf.Max(_size.x, _size.y, _size.z, SizeMin);
        return new Vector3(x, y, -z) * DistanceMultiplier * sizeMax;
    }

    private void Update()
    {
        Vector3 input = new Vector3 (-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        _orbitLocalRotation += input * MouseSensitivity * Time.unscaledDeltaTime;

        _orbitLocalRotation = new Vector3(
            Mathf.Clamp(_orbitLocalRotation.x, -85f, 85f),
            _orbitLocalRotation.y,
            _orbitLocalRotation.z);
    }

    public void Follow(Transform obj)
    {
        FollowObject = obj;
        SetMouseStatus(false);
        _orbitLocalRotation = obj.transform.rotation.eulerAngles;
        
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
        if (_collider)
        {
            Quaternion rot = Quaternion.Euler(_orbitLocalRotation);
            Vector3 position = _collider.transform.position + _collider.transform.rotation * _colCenter + rot * ComputeLocalOffset();
            Gizmos.DrawLine(_collider.transform.position + _collider.transform.rotation * _colCenter, _collider.transform.position + _collider.transform.rotation * _colCenter + rot * ComputeLocalOffset());
            Gizmos.DrawSphere(position, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(-AngleToObject, 0f, 0f) * transform.forward * 5f);
        }
    }
}
