using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    public Health Target;
    public float DamagePerSecond;
    public DamageMatrix.Damage DamageType;

    private void FixedUpdate()
    {
        Target.TakeDamage(new DamageInfo(DamagePerSecond * Time.fixedDeltaTime, DamageType, transform.position, Vector3.forward));
    }
}
