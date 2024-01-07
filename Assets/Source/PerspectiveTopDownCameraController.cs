using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveTopDownCameraController : TopDownCameraController
{
    public Vector2 HeightMinMax;
    public Vector2 PitchMinMax;

    protected override void UpdateCamera(float df)
    {
        Vector3 position = CurrentPosition;
        float height = Mathf.Lerp(HeightMinMax.x, HeightMinMax.y, ZoomLevel);
        float pitch = Mathf.Lerp(PitchMinMax.x, PitchMinMax.y, ZoomLevel);
        Quaternion rotation = Quaternion.Euler(pitch, CurrentAngle + 90f, 0f);
        Vector3 direction = rotation * (Vector3.back * height);

        transform.position = position + direction;
        transform.rotation = rotation;
    }
}
