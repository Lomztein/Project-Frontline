using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WeaponDPSMultiplierOverride : MonoBehaviour, IWeaponDPSOverride
{
    public IWeapon Weapon => GetWeapon();
    private IWeapon _weapon;

    public float Multiplier = 1f;

    private IWeapon GetWeapon ()
    {
        if (_weapon == null)
            _weapon = GetComponent<IWeapon>();
        return _weapon;
    }

    public float GetDPS ()
    {
        return Weapon.GetDPS() * Multiplier;
    }
}
