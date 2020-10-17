using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ControllableObject;
    public GameObject TurretGO;
    public ITurret Turret;
    public GameObject WeaponGO;
    public IWeapon Weapon;
    private IControllable _controllable;

    private void Awake()
    {
        if (ControllableObject)
        {
            Control(ControllableObject);
        }
    }

    public void Control (GameObject obj)
    {
        if (obj)
        {
            _controllable = obj.GetComponentInChildren<IControllable>();
        }
    }

    public void Release()
    {
        _controllable = null;
        Turret = null;
        Weapon = null;
    }

    void Update()
    {
        if (_controllable != null)
        {
            _controllable.Accelerate(Input.GetAxis("Vertical"));
            _controllable.Turn(Input.GetAxis("Horizontal"));
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ITarget target = new PositionTarget(ray.GetPoint(1000));

        if (Turret != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Turret.AimTowards(hit.point);
                target = hit.collider.gameObject.CompareTag("Terrain") ? target : new ColliderTarget(hit.collider);
            }
            else
            {
                Turret.AimTowards(ray.GetPoint(1000f));
            }
        }
        if (Weapon != null && Input.GetMouseButton(0))
        {
            Weapon.TryFire(target);
        }
    }
}
