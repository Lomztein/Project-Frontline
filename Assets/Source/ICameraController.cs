using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    void Pan(Vector2 movement);

    void Rotate(Vector2 rotation);

    void Zoom(float amount);

    void LookAt(Vector3 position);

    void TransitionFrom(Vector3 position, Quaternion rotation);
}
