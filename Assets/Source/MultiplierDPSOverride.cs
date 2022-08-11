using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplierDPSOverride : DPSOverride
{
    public Unit Unit;
    public float Multiplier;

    public override float GetDPS(IWeapon weapon)
    {
        return weapon.GetDPS() * Multiplier;
    }
}
