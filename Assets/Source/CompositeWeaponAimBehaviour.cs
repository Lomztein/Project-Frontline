using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class CompositeWeaponAimBehaviour : WeaponAimBehaviour
{
    [SerializeReference, SR]
    public WeaponAimBehaviour[] Behaviours;

    public override void SetAim(IWeapon weapon, float factor)
    {
        foreach (var behaviour in Behaviours)
        {
            behaviour.SetAim(weapon, factor);
        }
    }
}
