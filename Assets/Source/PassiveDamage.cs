using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDamage : MonoBehaviour
{
    public float InitialDamage;
    public float DamagePerSecond;
    public DamageMatrix.Damage DamageType;

    private void Start()
    {
        GetComponent<Health>().TakeDamage(new DamageInfo(InitialDamage, DamageType, transform.position, transform.forward));
    }

    private void FixedUpdate()
    {
        GetComponent<Health>().TakeDamage(new DamageInfo(DamagePerSecond, DamageType, transform.position, transform.forward));
    }
}
