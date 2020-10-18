using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBarrelChargingWeaponAnimator : MonoBehaviour
{
    public ChargingWeapon Weapon;
    public Transform RotatingTransform;

    public Vector3 RotationPerSecond;

    private void FixedUpdate()
    {
        RotatingTransform.Rotate(RotationPerSecond * Time.fixedDeltaTime * (Weapon.CurrentChargeTime / Weapon.MaxChargeTime));
    }
}
