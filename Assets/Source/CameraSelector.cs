using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public GameObject[] CameraObjects;
    private Camera[] _cameras;
    private ICameraController[] _cameraControllers;
    public Camera[] Cameras => _cameras;
    public ICameraController[] CameraControllers => _cameraControllers;
    public int SelectedIndex { get; private set; }

    public GameObject CurrentCameraObject => CameraObjects[SelectedIndex];
    public Camera CurrentCamera => _cameras[SelectedIndex];
    public ICameraController CurrentCameraController => _cameraControllers[SelectedIndex];

    private void Awake()
    {
        _cameras = CameraObjects.Select(x => x.GetComponent<Camera>()).ToArray();
        _cameraControllers = CameraObjects.Select(x => x.GetComponent<ICameraController>()).ToArray();
    }

    private void Start()
    {
        SelectCamera(SelectedIndex);
    }

    public void SetViewport(Rect viewport)
    {
        foreach (var camera in Cameras)
        {
            camera.rect = viewport;
        }
    }

    public bool CurrentIs<T>(out T controller) where T : class, ICameraController
    {
        if (CurrentCameraController != null && CurrentCameraController is T cam)
        {
            controller = cam;
            return true;
        }
        controller = null;
        return false;
    }

    public T CurrentAs<T>() where T : class, ICameraController
    {
        if (CurrentIs(out T controller))
        {
            return controller;
        }
        return null;
    }

    public void SelectCamera(int index)
    {
        ITransitionableCameraController sourceTrans = CurrentAs<ITransitionableCameraController>();

        Vector3 pos = CurrentCameraObject.transform.position;
        Quaternion rot = CurrentCameraObject.transform.rotation;
        CurrentCameraObject.SetActive(false);
        SelectedIndex = index % Cameras.Length;
        if (SelectedIndex < 0f) SelectedIndex = Cameras.Length - 1;
        CurrentCameraObject.SetActive(true);

        if (sourceTrans != null && CurrentIs(out ITransitionableCameraController transCam))
        {
            transCam.TransitionFrom(sourceTrans.GetTransitionStartPosition(), sourceTrans.GetTransitionStartRotation());
        }
    }


    public void Next ()
        => SelectCamera(SelectedIndex + 1);

    public void Prev ()
        => SelectCamera(SelectedIndex - 1);
}
