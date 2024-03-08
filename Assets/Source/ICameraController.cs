using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shared base interface for easier management.
public interface ICameraController 
{
    public string GetName();
}

public interface IMovableCameraController : ICameraController
{
    void Pan(Vector2 movement);

    void Rotate(Vector2 rotation);

    void Reset();
}

public interface IZoomableCameraController : ICameraController
{
    void Zoom(float amount);

    void ResetZoom();
}

public interface ISettableCameraController : ICameraController
{
    public void LookAt(Vector3 position);
}

public interface ITransitionableCameraController : ICameraController
{
    public Vector3 GetTransitionStartPosition();
    public Quaternion GetTransitionStartRotation();

    public void TransitionFrom(Vector3 position, Quaternion rotation);
}

public interface ICompositeCameraController : ICameraController
{
    public bool Change(int value);
}