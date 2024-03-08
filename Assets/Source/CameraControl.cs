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
    public InputAction CameraReset;
    public InputAction CameraToHQ;
    public InputAction CameraToFrontline;

    private float _cameraResetBeginTime;
    public float CameraResetTapLimit = 0.1f;

    private void Start()
    {
        PlayerHandler.PlayerInput.onControlsChanged += OnUpdate;
        OnUpdate(PlayerHandler.PlayerInput);
    }

    private void OnUpdate(PlayerInput input)
    {
        if (CameraPan != null)
        {
            CameraReset.performed -= CameraReset_Action;
            CameraReset.canceled -= CameraReset_Action;
        }

        CameraPan = input.actions["CameraPan"];
        CameraRotate = input.actions["CameraRotate"];
        CameraZoom = input.actions["CameraZoom"];
        CameraChange = input.actions["CameraChange"];
        CameraReset = input.actions["CameraReset"];
        CameraToHQ = input.actions["CameraToHQ"];
        CameraToFrontline = input.actions["CameraToFrontline"];

        CameraReset.performed += CameraReset_Action;
        CameraReset.canceled += CameraReset_Action;
    }

    private void CameraReset_Action(InputAction.CallbackContext ctx)
    {
        if (CameraSelector.CurrentIs(out IMovableCameraController movable))
        {
            if (ctx.phase == InputActionPhase.Canceled && ctx.duration < CameraResetTapLimit)
            {
                movable.Reset();
            }

            Debug.Log(ctx.phase);
            if (PlayerHandler.PlayerInputType == PlayerHandler.InputType.MouseAndKeyboard)
            {
                if (ctx.phase == InputActionPhase.Performed)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                if (ctx.phase == InputActionPhase.Canceled)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }

    private void Update()
    {
        Vector2 pan = CameraPan.ReadValue<Vector2>() * Time.deltaTime;
        Vector2 rotate = CameraRotate.ReadValue<Vector2>() * Time.deltaTime;
        float zoom = CameraZoom.ReadValue<float>() * Time.deltaTime;

        bool change = CameraChange.triggered;
        bool reset = CameraReset.triggered;
        bool toHq = CameraToHQ.triggered;
        bool toFrontline = CameraToFrontline.triggered;

        if (CameraSelector.CurrentIs(out IMovableCameraController movable))
        {
            movable.Pan(pan);
            movable.Rotate(rotate);
        }

        if (CameraSelector.CurrentIs(out IZoomableCameraController zoomable))
        {
            zoomable.Zoom(zoom);
        }

        if (CameraSelector.CurrentIs(out ISettableCameraController settable))
        {
            if (toHq)
            {
            settable.LookAt(GetHQPosition());
            }

            if (toFrontline)
            {
            settable.LookAt(GetFrontlinePosition());
            }
        }

        if (change)
        {
            int value = Mathf.RoundToInt(Mathf.Sign(CameraChange.ReadValue<float>()));
            ICompositeCameraController composite = CameraSelector.CurrentAs<ICompositeCameraController>();
            if (composite != null)
            {
                if (composite.Change(value))
                {
                    value = 0;
                }
            }

            if (value != 0)
            {
                CameraSelector.SelectCamera(CameraSelector.SelectedIndex + value);
            }
        }

        if (reset && Time.time > _cameraResetBeginTime + CameraResetTapLimit)
        {
        }

    }

    public void MoveToHQ ()
    {
        if (CameraSelector.CurrentIs(out ISettableCameraController settable))
        {
            settable.LookAt(GetHQPosition());
        }
    }

    private Vector3 GetHQPosition()
        => PlayerHandler.PlayerCommander.Fortress.position;

    private Vector3 GetFrontlinePosition()
        => PlayerHandler.PlayerCommander.Frontline.Position;
}
