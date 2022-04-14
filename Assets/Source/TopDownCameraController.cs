using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TopDownCameraController : MonoBehaviour
{
    public Vector2 PanSpeedMinMax;
    public float PanMargin;

    private float _targetZoomLevel = 1f;
    private float _zoomLevel = 1f;

    public float ZoomSpeed;
    public float ZoomLerpSpeed;

    private void Update()
    {
        Vector3 direction = GetPanDirection();
        float panSpeed = Mathf.Lerp(PanSpeedMinMax.x, PanSpeedMinMax.y, _zoomLevel);
        transform.position += panSpeed * Time.deltaTime * (Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * direction);

        _targetZoomLevel += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        _targetZoomLevel = Mathf.Clamp01(_targetZoomLevel);

        _zoomLevel = Mathf.Lerp(_zoomLevel, _targetZoomLevel, ZoomLerpSpeed * Time.deltaTime);

        UpdateZoom(_zoomLevel);
    }

    protected abstract void UpdateZoom(float zoomLevel);

    private Vector3 GetPanDirection ()
    {
        Vector2 mousePos = Input.mousePosition;
        float hor = 0f;
        float ver = 0f;

        if (mousePos.x < PanMargin) hor = -1;
        if (mousePos.x > Screen.width - PanMargin) hor = 1;

        if (mousePos.y < PanMargin) ver = -1;
        if (mousePos.y > Screen.height - PanMargin) ver = 1;

        return new Vector3(hor, 0f, ver);
    }
}
