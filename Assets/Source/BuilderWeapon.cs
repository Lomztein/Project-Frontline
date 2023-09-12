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

    [SerializeField] private DamageModifier _modifier;
    public DamageModifier Modifier => _modifier;

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

    public int Ammo => _canFire ? 1 : 0;
    public int MaxAmmo => 1;


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
            Unit unit = newBuilding.GetComponent<Unit>();
            if (unit)
            {
                var weapons = unit.GetWeapons();
                foreach (var weapon in weapons)
                {
                    weapon.OnProjectile += Weapon_OnProjectile;
                    weapon.OnHit += Weapon_OnHit;
                    weapon.OnKill += Weapon_OnKill;
                    weapon.OnDoDamage += Weapon_OnDoDamage;
                    weapon.OnDamageDone += Weapon_OnDamageDone;
                }
            }
            OnFire?.Invoke(this);
            return true;
        }
        return false;
    }

    private void Weapon_OnProjectile(IWeapon arg1, Projectile arg2) => OnProjectile?.Invoke(arg1, arg2);
    private void Weapon_OnDoDamage(IWeapon arg1, Projectile arg2, IDamagable arg3, DamageInfo arg4) => OnDoDamage?.Invoke(arg1, arg2, arg3, arg4);
    private void Weapon_OnDamageDone(IWeapon arg1, Projectile arg2, IDamagable arg3, DamageInfo arg4) => OnDamageDone?.Invoke(arg1, arg2, arg3, arg4);
    private void Weapon_OnKill(IWeapon arg1, Projectile arg2, IDamagable arg3) => OnKill?.Invoke(arg1, arg2, arg3);
    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 arg4, Vector3 arg5) => OnHit?.Invoke(arg1, arg2, arg3, arg4, arg5);

    private void Recharge()
    {
        _canFire = true;
    }

    public void SetHitLayerMask(LayerMask mask)
    {
    }
}
