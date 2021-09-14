using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerCamera : MonoBehaviour
{
    public Transform FollowObject;

    public Vector3 PositionOffset;
    public Vector3 LookPositionOffset;
    public float MouseSensitivity = 10f;
    public GameObject TargetImage;

    public float PositionLerpSpeed;
    public float RotationLerpSpeed;

    protected Vector3 TargetUp => FollowObject.up;

    private Vector2 _orbitRotation;
    private Vector3 _targetPosition;

    virtual protected void FixedUpdate()
    {
        if (FollowObject)
        {
            Quaternion orbitRotation = Quaternion.Euler(_orbitRotation);
            _targetPosition = Vector3.Lerp(_targetPosition, FollowObject.transform.position, PositionLerpSpeed * Time.deltaTime);

            transform.position = orbitRotation * PositionOffset + _targetPosition;
            transform.rotation = Quaternion.LookRotation((LookPositionOffset + FollowObject.transform.position) - transform.position, TargetUp);
        }
    }

    private void Update()
    {
        Vector2 input = new Vector3 (-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        _orbitRotation += input * MouseSensitivity * Time.unscaledDeltaTime;
    }

    public void Follow (Transform obj)
    {
        FollowObject = obj;
        SetMouseStatus(false);
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
        TargetImage.SetActive(!enabled);
    }

    protected virtual void OnDrawGizmos()
    {
        if (FollowObject)
        {
            Gizmos.DrawLine(FollowObject.transform.position + PositionOffset, FollowObject.transform.position + LookPositionOffset);
            Gizmos.DrawSphere(FollowObject.transform.position + PositionOffset, 0.5f);
        }
    }
}
