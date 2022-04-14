using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ControllableObject;

    public List<ITurret> Turrets = new List<ITurret>();
    public List<IWeapon> Weapons = new List<IWeapon>();
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
        Turrets.Clear();
        Weapons.Clear();
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

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Turrets.ForEach(x => x.AimTowards(hit.point));
            target = hit.collider.gameObject.CompareTag("Terrain") ? target : new ColliderTarget(hit.collider);
        }
        else
        {
            Turrets.ForEach(x => x.AimTowards(ray.GetPoint(1000f)));
        }

        for (int i = 0; i < Mathf.Min(Weapons.Count, 2); i++)
        {
            if (Input.GetMouseButton(i))
            {
                Debug.Log(Weapons[i]);
                Weapons[i].TryFire(target);
            }
        }
    }
}
