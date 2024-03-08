using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class OrbitCamera : MonoBehaviour
{
    public Transform FollowObject;

    public float AngleToObject = 20;
    public float DistanceMultiplier = 2f;
    public float SizeMin = 4f;

    public float MouseSensitivity = 10f;
    public float PositionLerpSpeed;

    public Vector3 OrbitRotation = new Vector3(0f, 0f, 0f);

    private Vector3 _size;
    private Vector3 _colCenter;
    private Vector3 _center;
    private Vector3 _targetPosition;

    private Collider _collider;

    virtual protected void FixedUpdate()
    {
        if (FollowObject)
        {
            Quaternion rot = Quaternion.Euler(OrbitRotation);

            _targetPosition = _center + rot * ComputeLocalOffset();
            transform.position = _targetPosition;
            transform.rotation = rot;

            _center = Vector3.Lerp(_center, FollowObject.transform.position + FollowObject.transform.rotation * _colCenter, PositionLerpSpeed * Time.fixedDeltaTime);
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
        OrbitRotation = new Vector3(
            Mathf.Clamp(OrbitRotation.x, -85f, 85f),
            OrbitRotation.y,
            OrbitRotation.z);
    }

    public void Rotate(Vector2 movement, float dt)
    {
        OrbitRotation += (Vector3)movement * MouseSensitivity * dt;
    }

    public void Rotate(Vector2 movement)
    {
        Rotate(movement, Time.unscaledDeltaTime);
    }

    public void Follow(Transform obj)
    {
        FollowObject = obj;

        Bounds bounds = UnityUtils.ComputeMinimallyBoundingBox(obj.gameObject);
        _size = bounds.size;
        _colCenter = bounds.center - FollowObject.transform.position;
    }

    protected virtual void OnDrawGizmos()
    {
        if (_collider)
        {
            Quaternion rot = Quaternion.Euler(OrbitRotation);
            Vector3 position = _collider.transform.position + _collider.transform.rotation * _colCenter + rot * ComputeLocalOffset();
            Gizmos.DrawLine(_collider.transform.position + _collider.transform.rotation * _colCenter, _collider.transform.position + _collider.transform.rotation * _colCenter + rot * ComputeLocalOffset());
            Gizmos.DrawSphere(position, 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Quaternion.Euler(-AngleToObject, 0f, 0f) * transform.forward * 5f);
        }
    }
}
