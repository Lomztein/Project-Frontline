using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPositionLerpChargingWeaponAnimator : MonoBehaviour
{
    public ChargingWeapon Weapon;
    public Transform Transform;

    public Vector3 From;
    public Vector3 To;
    public float LerpSpeed = 50;

    private void FixedUpdate()
    {
        float factor = Weapon.CurrentChargeTime / Weapon.MaxChargeTime;
        Vector3 targetPos = Vector3.Lerp(From, To, factor);
        Transform.localPosition = Vector3.Lerp(Transform.localPosition, targetPos, Time.fixedDeltaTime * LerpSpeed);
    }
}
