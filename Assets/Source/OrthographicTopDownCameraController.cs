using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicTopDownCameraController : TopDownCameraController
{
    public Camera Camera;
    public Vector2 SizeMinMax;
    public float Height = 200;

    protected override void UpdateCamera(float dt)
    {
        Camera.orthographicSize = Mathf.Lerp(SizeMinMax.x, SizeMinMax.y, ZoomLevel);
        transform.position = CurrentPosition + Vector3.up * Height;
        transform.rotation = Quaternion.Euler(90f, CurrentAngle, -90f);
    }
}
