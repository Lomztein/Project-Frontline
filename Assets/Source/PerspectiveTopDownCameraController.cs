using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveTopDownCameraController : TopDownCameraController
{
    public Vector2 HeightMinMax;
    public Vector2 PitchMinMax;

    protected override void UpdateZoom(float zoomLevel)
    {
        Vector3 position = transform.position;
        position.y = Mathf.Lerp(HeightMinMax.x, HeightMinMax.y, zoomLevel);
        transform.position = position;

        float pitch = Mathf.Lerp(PitchMinMax.x, PitchMinMax.y, zoomLevel);
        transform.rotation = Quaternion.Euler(pitch, 90f, 0f);
    }
}
