using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    public float MaxHealth;
    public DamageMatrix.Armor ArmorType;
    private bool _isDead;
    public float CurrentHealth { get; private set; }
    public bool DestroyOnDeath = true;
    public GameObject Debris;
    public float DebrisLife;

    public event Action<DamageInfo> OnTakeDamage;
    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public float TakeDamage (DamageInfo info)
    {
        OnTakeDamage?.Invoke(info);
        float dmg = info.Damage * DamageMatrix.GetDamageFactor(info.Type, ArmorType);
        CurrentHealth -= dmg;
        OnDamageTaken?.Invoke(dmg);
        if (CurrentHealth <= 0f && !_isDead)
        {
            Die();
            OnDeath?.Invoke();
            _isDead = true;
        }
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        return CurrentHealth;
    }

    public void Heal (float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void Revive ()
    {
        _isDead = false;
        CurrentHealth = MaxHealth;
    }

    private void Die()
    {
        if (DestroyOnDeath)
        {
            Destroy(gameObject);
        }

        if (Debris)
        {
            GameObject d = Instantiate(Debris, transform.position, transform.rotation);
            
            foreach (ParticleSystem system in d.GetComponentsInChildren<ParticleSystem>())
                system.Play();

            Destroy(d, DebrisLife);
        }
    }
}
