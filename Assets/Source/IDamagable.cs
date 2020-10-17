using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    float TakeDamage(DamageArmorMapping.Damage damageType, float damage);
}
