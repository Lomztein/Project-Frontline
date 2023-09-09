using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArmorType : ScriptableObject
{
    public abstract float MitigateDamage(float damage, DamageInfo info, DamageType type);
}
