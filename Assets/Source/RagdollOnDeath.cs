using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    public Health Health;
    public GameObject Root;

    public Ragdoll Ragdoll;
    public Transform RagdollRoot;

    private DamageInfo _lastDamage;

    void Start()
    {
        Health.OnDeath += Health_OnDeath;
        Health.OnTakeDamage += Health_OnTakeDamage;
    }

    private void Health_OnTakeDamage(DamageInfo obj)
    {
        _lastDamage = obj;
    }

    private void Health_OnDeath()
    {
        Ragdoll.enabled = true;
        RagdollRoot.SetParent(null);
        if (_lastDamage != null)
        {
            Ragdoll.AddForce(_lastDamage.Point, _lastDamage.Direction * _lastDamage.Damage);
        }
        Destroy(Root.gameObject);
        Destroy(RagdollRoot.gameObject, 10f);
    }

    private void Freeze ()
    {
        Ragdoll.enabled = false;
    }
}
