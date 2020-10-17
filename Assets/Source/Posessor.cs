using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posessor : MonoBehaviour
{
    private GameObject _currentPosessed;

    public Camera MainCamera;
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
            CameraController.FollowObject = target.transform;

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
                    var toPosess = hit.collider.GetComponentInParent<IController>() as Component;
                    if (toPosess == null)
                    {
                        toPosess = hit.collider.GetComponentInChildren<IController>() as Component;
                    }
                    if (toPosess)
                    {
                        Posess(toPosess.gameObject);
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
        Controller.Release();
    }
}
