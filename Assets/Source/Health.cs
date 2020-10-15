using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float MaxHealth;
    private float _currentHealth;
    public GameObject Debris;
    public float DebrisLife;

    private void Awake()
    {
        _currentHealth = MaxHealth;
    }

    public void TakeDamage (float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Destroy(Instantiate(Debris, transform.position, transform.rotation), DebrisLife);
    }
}
