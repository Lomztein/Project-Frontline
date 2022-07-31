using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Focuses targets with the most health that can be killed with x amount of shots from a weapon.
public class SurgicalFocusAIControllerModifier : AIControllerModifier
{
    public Weapon Weapon;
    public int Shots;
    public float HealthWeight = 10f;

    public override void OnInitialized(AIController controller)
    {
        controller.SetTargetEvaluator(EvaluateTarget);
    }

    private float EvaluateTarget(Vector3 pos, GameObject obj)
    {
        Health targetHealth = obj.GetComponentInParent<Health>();
        if (CanKillInSingleVolley(targetHealth))
            return -Vector3.SqrMagnitude(obj.transform.position - pos) + Mathf.Pow(targetHealth.CurrentHealth * HealthWeight, 2);
        else
        {
            return float.MinValue;
        }
    }

    private bool CanKillInSingleVolley(Health health)
    {
        float expectedDamage = DamageMatrix.GetDamageFactor(Weapon.DamageType, health.ArmorType) * Shots * Weapon.Amount * Weapon.Damage;
        return expectedDamage > health.CurrentHealth;
    }

}
