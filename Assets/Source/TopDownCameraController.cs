using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TopDownCameraController : MonoBehaviour
{
    public float PanSpeed;
    public float PanMargin;

    private void Update()
    {
        Vector3 direction = GetPanDirection();
        transform.position += PanSpeed * Time.deltaTime * (Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * direction);
        Zoom(Input.GetAxis("Mouse ScrollWheel"), Time.deltaTime);
    }

    protected abstract void Zoom(float amount, float deltaTime);

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
