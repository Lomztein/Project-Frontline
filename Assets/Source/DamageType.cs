using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamageType : ScriptableObject
{
    public abstract float ModifyDamage(float damage, DamageInfo info, ArmorType armor);
}
