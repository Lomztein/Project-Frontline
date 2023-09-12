using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDamage : MonoBehaviour
{
    public float InitialDamage;
    public float DamagePerSecond;
    public float DamagePerSecondAcceleration;
    public DamageModifier Modifier;

    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        _health.TakeDamage(new DamageInfo(InitialDamage, Modifier, transform.position, transform.forward, this, _health));
    }

    private void FixedUpdate()
    {
        _health.TakeDamage(new DamageInfo(DamagePerSecond * Time.fixedDeltaTime, Modifier, transform.position, transform.forward, this, _health));
        DamagePerSecond += DamagePerSecondAcceleration * Time.fixedDeltaTime;
    }
}
