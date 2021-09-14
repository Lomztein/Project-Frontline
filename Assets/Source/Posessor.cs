using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class Posessor : MonoBehaviour
{
    private GameObject _currentPosessed;

    public Camera MainCamera;
    public SimpleCameraController MainCameraController;

    public FollowerCamera CameraController;
    private Camera _followCamera;

    public PlayerController Controller;

    private void Awake()
    {
        _followCamera = CameraController.GetComponent<Camera>();
    }

    public void Posess (GameObject target)
    {
        IControllable controllable = target.GetComponentInChildren<IControllable>();
        ITurret turret = target.GetComponentInChildren<ITurret>();
        IWeapon weapon = target.GetComponentInChildren<IWeapon>();

        if (controllable != null || turret != null || weapon != null)
        {
            MainCamera.enabled = false;
            _followCamera.enabled = true;
            CameraController.Follow(target.transform);
            MainCameraController.enabled = false;

            Controller.Turret = turret;
            Controller.Weapon = weapon;
            Controller.Control(target);

            _currentPosessed = target;
            _currentPosessed.GetComponentInChildren<IController>().Enabled = false;
        }
    }

    private void Update()
    {
        if (_currentPosessed == null)
        {
            if (_followCamera.enabled == true)
            {
                if (!IsInvoking())
                {
                    Invoke("Reset", 3f);
                }
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                if (Input.GetMouseButtonDown (0))
                {
                    var controllable = hit.collider.GetComponentInParent<IControllable>() as Component;
                    if (controllable == null)
                    {
                        controllable = hit.collider.GetComponentInParent<IController>() as Component;
                        if (controllable == null)
                        {
                            controllable = hit.collider.GetComponentInChildren<IControllable>() as Component;
                            if (controllable == null)
                            {
                                controllable = hit.collider.GetComponentInChildren<IControllable>() as Component;
                                // Are you proud of me? I'm proud of me.
                            }
                        }
                    }
                    if (controllable)
                    {
                        Posess(controllable.gameObject);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Reset();
            }
        }
    }

    private void Reset()
    {
        if (_currentPosessed)
        {
            _currentPosessed.GetComponentInChildren<IController>().Enabled = true;
        }

        MainCamera.enabled = true;
        _followCamera.enabled = false;
        _currentPosessed = null;
        CameraController.StopFollow();
        MainCameraController.enabled = true;
        Controller.Release();
    }

}
