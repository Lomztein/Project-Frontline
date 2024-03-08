using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class ObjectDisplayScene : MonoBehaviour
{
    public Transform DisplayRoot;
    public OrbitCamera OrbitCamera;
    public StackOfRigidbodies StackOfRigidbodies;
    private Unit _currentUnit;
    public float ForceMult = 5f;

    public GameObject Display(GameObject prefab)
    {
        Clear();

        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(DisplayRoot); 
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localRotation = Quaternion.identity;

        OrbitCamera.Follow(newObject.transform);
        
        if (newObject.TryGetComponent(out Unit unit)) {
            HandleUnit(unit);
        }
        if (StackOfRigidbodies)
        {
            StackOfRigidbodies.Reset();
        }

        return newObject;
    }

    private void HandleUnit(Unit unit)
    {
        if (_currentUnit)
        {
            foreach (var weapon in _currentUnit.GetWeapons())
            {
                weapon.OnHit -= Weapon_OnHit;
            }
        }
        _currentUnit = unit;
        foreach (var weapon in _currentUnit.GetWeapons())
        {
            weapon.OnHit += Weapon_OnHit;
        }

    }

    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 point, Vector3 normal)
    {
        if (arg3.TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(arg2.transform.forward * arg2.Damage);

            float radius = Mathf.Max(1f, Mathf.Log(arg2.Damage, 2f));
        Collider[] cols = Physics.OverlapSphere(point, radius);
        foreach (Collider col in cols)
        {
            if (col.TryGetComponent(out rigidbody))
            {
                float factor = 1f - Mathf.Clamp01(Vector3.Distance(col.transform.position, point) / radius);
                float dot = Mathf.Clamp(-Vector3.Dot(arg2.transform.forward, (point - col.transform.position).normalized), 0, 1f);
                rigidbody.AddExplosionForce(arg2.Damage * factor * dot * ForceMult, point, radius);
            }
        }
    }

    private void FixedUpdate()
    {
        if (StackOfRigidbodies && _currentUnit && _currentUnit.TryGetComponent<AIController>(out var controller) && controller.Turret != null)
        {
            controller.Turret.AimTowards(StackOfRigidbodies.Center);
        }
    }

    public void Clear ()
    {
        foreach (Transform child in DisplayRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
