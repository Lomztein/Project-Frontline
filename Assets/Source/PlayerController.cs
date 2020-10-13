using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ControllableObject;
    public Turret Turret;
    public Weapon Weapon;
    private IControllable _controllable;

    private void Awake()
    {
        _controllable = ControllableObject.GetComponent<IControllable>();
    }

    void Update()
    {
        _controllable.Accelerate(Input.GetAxis("Vertical"));
        _controllable.Turn(Input.GetAxis("Horizontal"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Turret.AimTowards(hit.point);
        }
        else
        {
            Turret.AimTowards(ray.GetPoint(1000f));
        }
        if (Input.GetMouseButton(0))
        {
            Weapon.TryFire();
        }
    }
}
