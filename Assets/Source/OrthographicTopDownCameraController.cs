using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicTopDownCameraController : TopDownCameraController
{
    public Camera Camera;

    public Vector2 SizeMinMax;
    public float CurrentZoom = 1f;
    private float _targetZoom = 1f;
    public float ZoomSpeed;
    public float ZoomLerpSpeed;

    protected override void Zoom(float amount, float deltaTime)
    {
        _targetZoom += amount * ZoomSpeed * deltaTime;
        _targetZoom = Mathf.Clamp01(_targetZoom);
        CurrentZoom = Mathf.Lerp(CurrentZoom, _targetZoom, ZoomLerpSpeed * deltaTime);
        Camera.orthographicSize = Mathf.Lerp(SizeMinMax.x, SizeMinMax.y, CurrentZoom);
    }
}
