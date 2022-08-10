using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicTopDownCameraController : TopDownCameraController
{
    public Camera Camera;
    public Vector2 SizeMinMax;

    protected override void UpdateZoom(float zoomLevel)
    {
        Camera.orthographicSize = Mathf.Lerp(SizeMinMax.x, SizeMinMax.y, zoomLevel);
        transform.rotation = Quaternion.Euler(90f, 0f, -90f);
    }
}
