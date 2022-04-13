using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveTopDownCameraController : TopDownCameraController
{
    public Vector2 HeightMinMax;
    public Vector2 PitchMinMax;

    private float _targetZoomLevel = 1f;
    public float ZoomLevel = 1f;
    public float ZoomSpeed;
    public float ZoomLerpSpeed;

    protected override void Zoom(float amount, float deltaTime)
    {
        _targetZoomLevel += amount * ZoomSpeed * deltaTime;
        _targetZoomLevel = Mathf.Clamp01(_targetZoomLevel);
        ZoomLevel = Mathf.Lerp(ZoomLevel, _targetZoomLevel, ZoomLerpSpeed * deltaTime);

        Vector3 position = transform.position;
        position.y = Mathf.Lerp(HeightMinMax.x, HeightMinMax.y, ZoomLevel);
        transform.position = position;

        float pitch = Mathf.Lerp(PitchMinMax.x, PitchMinMax.y, ZoomLevel);
        transform.rotation = Quaternion.Euler(pitch, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        Debug.Log(_targetZoomLevel);
    }
}
