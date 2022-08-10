using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDamage : MonoBehaviour
{
    public float InitialDamage;
    public float DamagePerSecond;
    public float DamagePerSecondAcceleration;
    public DamageMatrix.Damage DamageType;

    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        _health.TakeDamage(new DamageInfo(InitialDamage, DamageType, transform.position, transform.forward));
    }

    private void FixedUpdate()
    {
        _health.TakeDamage(new DamageInfo(DamagePerSecond * Time.fixedDeltaTime, DamageType, transform.position, transform.forward));
        DamagePerSecond += DamagePerSecondAcceleration * Time.fixedDeltaTime;
    }
}
