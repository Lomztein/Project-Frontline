using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TopDownCameraController : MonoBehaviour, ICameraController
{
    public Vector2 PanSpeedMinMax;
    public float PanMargin;

    private float _targetZoomLevel = 1f;
    private float _zoomLevel = 1f;

    public float ZoomSpeed;
    public float ZoomLerpSpeed;

    public PlayerHandler Handler;

    private void Update()
    {
        if (Application.isFocused)
        {
            Vector3 direction = GetPanDirection();
            Pan(direction * Time.deltaTime);
            UpdateZoom(_zoomLevel);
        }
    }

    protected abstract void UpdateZoom(float zoomLevel);

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
        float panSpeed = Mathf.Lerp(PanSpeedMinMax.x, PanSpeedMinMax.y, _zoomLevel);
        Vector3 xzMovement = new Vector3(movement.x, 0f, movement.y);
        xzMovement = panSpeed * (Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * xzMovement);
        transform.position += xzMovement;
    }

    public void Rotate(Vector2 rotation)
    {
    }

    public void Zoom(float amount)
    {
        _targetZoomLevel += amount * ZoomSpeed;
        _targetZoomLevel = Mathf.Clamp01(_targetZoomLevel);

        _zoomLevel = Mathf.Lerp(_zoomLevel, _targetZoomLevel, ZoomLerpSpeed * Time.deltaTime);
    }

    public void LookAt(Vector3 position)
    {
        float y = transform.position.y;
        transform.position = position;
        transform.Translate(Vector3.back * y);
    }

    public void TransitionFrom(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}
