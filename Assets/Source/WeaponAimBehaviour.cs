using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class WeaponAimBehaviour
{
    public abstract void SetAim(IWeapon weapon, float factor);
}
