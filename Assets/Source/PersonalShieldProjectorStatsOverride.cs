using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalShieldProjectorStatsOverride : MonoBehaviour
{
    public bool DoOverrideSize;
    public float OverrideSize;
    public bool DoOverrideHealth;
    public float OverrideHealth;

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, OverrideSize);
    }
}
