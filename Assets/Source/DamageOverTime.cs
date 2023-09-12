using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public Health Target;
    public float DamagePerSecond;
    public DamageModifier Modifier;

    private void FixedUpdate()
    {
        Target.TakeDamage(new DamageInfo(DamagePerSecond * Time.fixedDeltaTime, Modifier, transform.position, Vector3.forward, this, Target));
    }
}
