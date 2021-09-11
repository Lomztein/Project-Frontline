using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    public Health Health;
    public GameObject Root;

    public Ragdoll Ragdoll;
    public Transform RagdollRoot;

    void Start()
    {
        Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath()
    {
        Ragdoll.enabled = true;
        RagdollRoot.SetParent(null);
        Destroy(Root.gameObject);
        Destroy(RagdollRoot.gameObject, 10f);
    }
}
