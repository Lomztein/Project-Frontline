using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnController : MonoBehaviour
{
    public BurnEffect Effect;
    public DamageOverTime DamageOverTime;
    public DestroyObject Destroyer;

    public void SetTarget (Collider target)
    {
        Effect.ApplyTo(target.gameObject);
        DamageOverTime.Target = target.GetComponentInParent<Health>();
    }
}
