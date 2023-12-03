﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    public float MaxHealth;
    public DamageModifier Modifier;
    private bool _isDead;
    public float CurrentHealth { get; private set; }
    public bool DestroyOnDeath = true;
    public float DestroyDelay;
    public GameObject Debris;
    public float DebrisLife;

    public event Action<Health, DamageInfo> OnTakeDamage;
    public event Action<Health, DamageInfo> OnDamageTaken;
    public event Action<Health, DamageInfo> OnHeal;
    public event Action<Health> OnDeath;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public float TakeDamage (DamageInfo info)
    {
        float h = CurrentHealth;
        OnTakeDamage?.Invoke(this, info);
        float dmg = info.GetDamage(Modifier);
        CurrentHealth -= dmg;
        info.DamageDone = Mathf.Clamp(h - CurrentHealth, -MaxHealth, MaxHealth);
        OnDamageTaken?.Invoke(this, info);
        if (CurrentHealth <= 0f && !_isDead)
        {
            Die();
            info.KilledTarget = true;
            OnDeath?.Invoke(this);
            _isDead = true;
        }
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        return CurrentHealth;
    }

    public void Heal (DamageInfo info)
    {
        float h = CurrentHealth;
        float dmg = info.GetDamage(Modifier);
        CurrentHealth += dmg;
        info.DamageDone = Mathf.Clamp(h - CurrentHealth, -MaxHealth, MaxHealth);
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        OnHeal?.Invoke(this, info);
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
            Destroy(gameObject, DestroyDelay);
        }

        if (Debris)
        {
            GameObject d = Instantiate(Debris, transform.position, transform.rotation);
            d.GetComponent<Effect>().Play();
            Destroy(d, DebrisLife);
        }
    }
}
