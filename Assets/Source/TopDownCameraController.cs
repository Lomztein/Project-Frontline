using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TopDownCameraController : MonoBehaviour, ICameraController
{
    public Vector2 PanSpeedMinMax;
    public float PanMargin;
    public float RotateSpeed;

    private Vector3 _targetPosition;
    public Vector3 CurrentPosition { get; private set; }
    public float PositionLerpSpeed = 10f;

    private float _targetAngle;
    protected float CurrentAngle { get; private set; }
    public float AngleLerpSpeed = 10f;

    private float _targetZoomLevel = 1f;
    protected float ZoomLevel { get; private set; }

    public float ZoomSpeed;
    public float ZoomLerpSpeed;


    public PlayerHandler Handler;

    private void Update()
    {
        if (Application.isFocused)
        {
            Vector3 direction = GetPanDirection();
            Pan(direction * Time.deltaTime);
            UpdateCamera(Time.deltaTime);

            ZoomLevel = Mathf.Lerp(ZoomLevel, _targetZoomLevel, ZoomLerpSpeed * Time.deltaTime);
            CurrentPosition = Vector3.Lerp(CurrentPosition, _targetPosition, PositionLerpSpeed * Time.deltaTime);
            CurrentAngle = Mathf.Lerp(CurrentAngle, _targetAngle, AngleLerpSpeed * Time.deltaTime);
        }
    }

    protected abstract void UpdateCamera(float dt);

    private Vector3 GetPanDirection ()
    {
        Vector2 mousePos = Handler.GetPointerScreenPosition();
        float hor = 0f;
        float ver = 0f;

        Rect screenRect = Handler.ViewportRectToScreenSpace();

        if (mousePos.x < screenRect.x + PanMargin) hor = -1;
        if (mousePos.x > screenRect.x + screenRect.width - PanMargin) hor = 1;

        if (mousePos.y < screenRect.y + PanMargin) ver = -1;
        if (mousePos.y > screenRect.y + screenRect.height - PanMargin) ver = 1;

        return new Vector3(hor, ver, 0f);
    }

    public void Pan(Vector2 movement)
    {
        float panSpeed = Mathf.Lerp(PanSpeedMinMax.x, PanSpeedMinMax.y, ZoomLevel);
        Vector3 xzMovement = new Vector3(movement.x, 0f, movement.y);
        xzMovement = panSpeed * (Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * xzMovement);
        _targetPosition += xzMovement;
    }

    public void Rotate(Vector2 rotation)
    {
        _targetAngle += -rotation.x * RotateSpeed;
    }

    public void Zoom(float amount)
    {
        _targetZoomLevel += amount * ZoomSpeed;
        _targetZoomLevel = Mathf.Clamp01(_targetZoomLevel);
    }

    public void LookAt(Vector3 position)
    {
        _targetPosition = position;
    }

    public void TransitionFrom(Vector3 position, Quaternion rotation)
    {
        _targetPosition = position.Flat();
        _targetAngle = 0f;
    }

    public void Reset()
    {
        _targetAngle = 0f;
    }
}
