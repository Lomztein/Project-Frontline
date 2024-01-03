using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.Rendering.DebugUI;

public class CameraControl : MonoBehaviour
{
    public PlayerHandler PlayerHandler;
    public CameraSelector CameraSelector;

    public InputAction CameraPan;
    public InputAction CameraRotate;
    public InputAction CameraZoom;
    public InputAction CameraChange;
    public InputAction CameraToHQ;
    public InputAction CameraToFrontline;

    private void Start()
    {
        PlayerHandler.PlayerInput.onControlsChanged += OnUpdate;
        OnUpdate(PlayerHandler.PlayerInput);
    }

    private void OnUpdate(PlayerInput input)
    {
        CameraPan = input.actions["CameraPan"];
        CameraRotate = input.actions["CameraRotate"];
        CameraZoom = input.actions["CameraZoom"];
        CameraChange = input.actions["CameraChange"];
        CameraToHQ = input.actions["CameraToHQ"];
        CameraToFrontline = input.actions["CameraToFrontline"];
    }

    private void Update()
    {
        Vector2 pan = CameraPan.ReadValue<Vector2>() * Time.deltaTime;
        Vector2 rotate = CameraRotate.ReadValue<Vector2>() * Time.deltaTime;
        float zoom = CameraZoom.ReadValue<float>() * Time.deltaTime;

        bool change = CameraChange.triggered && CameraChange.ReadValue<float>() > 0.5f;
        bool toHq = CameraToHQ.triggered;
        bool toFrontline = CameraToFrontline.triggered;

        ICameraController controller = CameraSelector.CurrentCameraController;
        if (controller != null )
        {
            controller.Pan(pan);
            controller.Rotate(rotate);
            controller.Zoom(zoom);

            if (toHq)
            {
                controller.LookAt(GetHQPosition());
            }

            if (toFrontline)
            {
                controller.LookAt(GetFrontlinePosition());
            }
        }

        if (change)
        {
            int value = Mathf.RoundToInt(Mathf.Sign(CameraChange.ReadValue<float>()));
            CameraSelector.SelectCamera(CameraSelector.SelectedIndex + value);
        }

    }

    public void MoveToHQ ()
    {
        ICameraController controller = CameraSelector.CurrentCameraController;
        controller.LookAt(GetHQPosition());
    }

    private Vector3 GetHQPosition()
        => PlayerHandler.PlayerCommander.Fortress.position;

    private Vector3 GetFrontlinePosition()
        => PlayerHandler.PlayerCommander.Frontline.Position;
}
