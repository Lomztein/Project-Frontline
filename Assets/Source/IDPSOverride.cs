using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DPSOverride : MonoBehaviour
{
    public abstract float GetDPS(IWeapon weapon);
}
