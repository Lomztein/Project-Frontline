using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    public Health Health;
    public GameObject Root;

    public Ragdoll Ragdoll;
    public Transform RagdollRoot;
    public Collider RootCollider;

    public float Sturdyness;

    private DamageInfo _lastDamage;

    void Start()
    {
        Health.OnDeath += Health_OnDeath;
        Health.OnTakeDamage += Health_OnTakeDamage;
        if (Sturdyness == 0) Sturdyness = Health.MaxHealth / 200; // Magic number for backwards compatability lol
    }

    private void Health_OnTakeDamage(Health health, DamageInfo obj)
    {
        _lastDamage = obj;
    }

    private void Health_OnDeath(Health health)
    {
        Ragdoll.enabled = true;
        RagdollRoot.SetParent(null);
        if (RootCollider)
        {
            RootCollider.enabled = false;
        }
        if (_lastDamage != null)
        {
            Ragdoll.AddForce(_lastDamage.Point, _lastDamage.Direction * _lastDamage.Damage / Sturdyness);
        }
        Destroy(Root.gameObject);
        Destroy(RagdollRoot.gameObject, 10f);
    }

    private void Freeze ()
    {
        Ragdoll.enabled = false;
    }
}
