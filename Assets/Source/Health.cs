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
    public event Action<DamageInfo> OnTakeDamage;
    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        _currentHealth = MaxHealth;
    }

    public float TakeDamage (DamageInfo info)
    {
        OnTakeDamage?.Invoke(info);
        float dmg = info.Damage * DamageArmorMapping.GetDamageFactor(info.Type, ArmorType);
        _currentHealth -= dmg;
        OnDamageTaken?.Invoke(dmg);
        if (_currentHealth <= 0f)
        {
            Die();
            OnDeath?.Invoke();
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
