using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    public float MaxHealth;
    public DamageArmorMapping.Armor ArmorType;
    private float _currentHealth;
    public GameObject Debris;
    public float DebrisLife;

    private void Awake()
    {
        _currentHealth = MaxHealth;
    }

    public float TakeDamage (DamageArmorMapping.Damage damageType, float damage)
    {
        _currentHealth -= damage * DamageArmorMapping.GetDamageFactor(damageType, ArmorType);
        if (_currentHealth <= 0f)
        {
            Die();
        }
        return _currentHealth;
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject d = Instantiate(Debris, transform.position, transform.rotation);
        foreach (ParticleSystem system in d.GetComponentsInChildren<ParticleSystem>())
        {
            system.Play();
        }
        Destroy(d, DebrisLife);
    }
}
