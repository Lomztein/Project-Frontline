using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderWeapon : MonoBehaviour, IWeapon, ITeamComponent, ICommanderComponent
{
    [SerializeField] private float _damage;
    public float Damage => _damage;
    [SerializeField] private float _firerate;
    public float Firerate => _firerate;
    [SerializeField] private float _speed;
    public float Speed => _speed;

    [SerializeField] private DamageMatrix.Damage _damageType;
    public DamageMatrix.Damage DamageType => _damageType;

    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDoDamage;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDamageDone;

    private Commander _commander;
    private TeamInfo _teamInfo;

    private bool _canFire = true;

    public GameObject BuildingPrefab;

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }

    public void SetTeam(TeamInfo team)
    {
        _teamInfo = team;
    }

    public bool CanFire()
    {
        return _canFire;
    }

    public float GetDPS()
    {
        return _damage * _firerate;
    }

    public bool TryFire(ITarget intendedTarget)
    {
        if (_canFire)
        {
            GameObject newBuilding = Instantiate(BuildingPrefab, transform.root.position, transform.root.rotation);
            if (_commander != null) _commander.AssignCommander(newBuilding);
            if (_teamInfo != null) _teamInfo.ApplyTeam(newBuilding);
            _canFire = false;
            Invoke(nameof(Recharge), 1f / Firerate);
            return true;
        }
        return false;
    }

    private void Recharge()
    {
        _canFire = true;
    }

    public void SetHitLayerMask(LayerMask mask)
    {
    }
}
