using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityTemplateProjects;

public class Posessor : MonoBehaviour
{
    private GameObject _currentPosessed;
    private GameObject _currentFirstPersonController;

    public GameObject MainCamera;

    public FollowerCamera CameraController;
    private Camera _followCamera;

    public PlayerController Controller;
    public GameObject FirstPersonControllerPrefab;

    public List<ITurret> _turrets;
    public List<IWeapon> _weapons;

    public event Action<GameObject> OnPosess;
    public event Action<GameObject> OnRelease;

    private void Awake()
    {
        _followCamera = CameraController.GetComponent<Camera>();
    }

    public void Posess (GameObject target)
    {
        IControllable controllable = target.GetComponentInChildren<IControllable>();
        AIController controller = target.GetComponentInChildren<AIController>();

        if (controllable != null && controller != null)
        {
            InfantryBody body = controllable as InfantryBody;

            bool success;
            if (body != null)
            {
                success = TryPosessAsFirstPerson(target, controller, body);
            }
            else
            {
                success = TryPosessAsThirdPerson(target, controller, controllable);
            }

            if (success)
            {
                MainCamera.SetActive(false);
                OnPosess?.Invoke(target);
            }
        }
    }

    private bool TryPosessAsThirdPerson (GameObject target, AIController controller, IControllable controllable)
    {
        // Default controls.
        _followCamera.gameObject.SetActive(true);
        _currentPosessed = target;
        CameraController.Follow(target.transform);

        if (controller.Turret != null)
        {
            Controller.Turrets.Add(controller.Turret);
        }
        Controller.Weapons.AddRange(controller.Weapons);
        Unit unit = target.GetComponent<Unit>();
        Array.ForEach(unit.GetWeapons().ToArray(), x => x.OnDamageDone += Weapon_OnDamageDone);
        Controller.Control(target);

        _currentPosessed.GetComponentInChildren<IController>().Enabled = false;
        return true;
    }

    private void Weapon_OnDamageDone(IWeapon arg1, Projectile arg2, IDamagable arg3, DamageInfo arg4)
    {
        Hitmarker.Create(arg4.Point, arg4.BaseDamage, arg4.DamageDone);
    }

    private bool TryPosessAsFirstPerson(GameObject target, AIController controller, InfantryBody inf)
    {
        if (TryConvertToFirstPerson(target, controller, inf))
        {
            _currentFirstPersonController = Instantiate(FirstPersonControllerPrefab, target.transform.position, target.transform.rotation);
            _currentFirstPersonController.transform.SetParent(target.transform.parent);
            target.transform.SetParent(_currentFirstPersonController.transform, true);

            Health health = target.GetComponentInChildren<Health>();
            health.OnDeath += DeathCam;

            FirstPersonController fpc = _currentFirstPersonController.GetComponentInChildren<FirstPersonController>();

            fpc.MoveSpeed = inf.MaxSpeed;
            fpc.SprintSpeed = inf.MaxSpeed * 1.25f;

            FirstPersonWeaponsController fpw = _currentFirstPersonController.GetComponentInChildren<FirstPersonWeaponsController>();
            fpw.WeaponPrefabs = controller.WeaponObjects.ToArray();
            fpw.UpdateWeapons();

            Array.ForEach(fpw.Weapons, x => x.OnDamageDone += Weapon_OnDamageDone);

            controller.Team.ApplyTeam(_currentFirstPersonController);
            _currentPosessed = target;

            return true;
        }
        return false;
    }

    private void DeathCam (Health health)
    {
        FirstPersonController fpc = health.GetComponentInParent<FirstPersonController>();
        fpc.Camera.transform.SetParent(null);
        Destroy(fpc.Camera, 3f);
        Reset();
    }

    private bool TryConvertFirstPerson(GameObject target, AIController controller, InfantryBody inf, bool value)
    {
        if (target)
        {
            var rig = target.GetComponentInChildren<SkinnedMeshRenderer>(true);
            if (rig)
            {
                rig.transform.parent.gameObject.SetActive(value); // Assume first smr is a child of the rig root.
                rig.gameObject.SetActive(value);

                inf.enabled = value;
                controller.enabled = value;

                var weaponObjects = controller.WeaponObjects;
                foreach (var wep in weaponObjects)
                {
                    wep.SetActive(value);
                }

                return true;
            }
        }
        return false;
    }
    private bool TryConvertToFirstPerson(GameObject target, AIController controller, InfantryBody inf)
        => TryConvertFirstPerson(target, controller, inf, false);

    private bool TryConvertFromFirstPerson(GameObject target, AIController controller, InfantryBody inf)
        => TryConvertFirstPerson(target, controller, inf, true);

    private void Update()
    {
        if (_currentPosessed == null)
        {
            if (_followCamera.gameObject.activeInHierarchy == true)
            {
                if (!IsInvoking())
                {
                    OnRelease?.Invoke(null);
                    Controller.Release();
                    Invoke(nameof(Reset), 3f);
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
            InfantryBody inf = _currentPosessed.GetComponentInChildren<InfantryBody>(true);
            if (inf)
            {
                _currentPosessed.transform.SetParent(_currentFirstPersonController.transform.parent);
                Destroy(_currentFirstPersonController);
                _currentFirstPersonController.transform.SetParent(null);
                _currentPosessed.GetComponent<Health>().OnDeath -= DeathCam;

                FirstPersonWeaponsController fpw = _currentFirstPersonController.GetComponentInChildren<FirstPersonWeaponsController>();
                Array.ForEach(fpw.Weapons, x => x.OnDamageDone -= Weapon_OnDamageDone);

                _currentFirstPersonController = null;
                TryConvertFromFirstPerson(_currentPosessed, _currentPosessed.GetComponent<AIController>(), inf);
            }
            else
            {
                AIController controller = _currentPosessed.GetComponentInChildren<AIController>();
                Unit unit = _currentPosessed.GetComponent<Unit>();
                Array.ForEach(unit.GetWeapons().ToArray(), x => x.OnDamageDone -= Weapon_OnDamageDone);
                controller.Enabled = true;
            }
        }

        MainCamera.SetActive(true);
        _followCamera.gameObject.SetActive(false);
        CameraController.StopFollow();
        Controller.Release();
        OnRelease?.Invoke(_currentPosessed);
        _currentPosessed = null;
    }
}
