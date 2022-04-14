using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Health))]
public class TargetableProjectile : MonoBehaviour, ITeamComponent
{
    public Health Health;
    public Projectile Projectile;

    public void SetTeam(TeamInfo team)
    {
        gameObject.layer = team.ProjectileGetLayer();
    }

    private void Awake()
    {
        Health.OnDeath += TargetableProjectile_OnDeath;
    }

    private void TargetableProjectile_OnDeath()
    {
        Projectile.End();
    }

    private void OnEnable()
    {
        Health.Revive();
    }
}
