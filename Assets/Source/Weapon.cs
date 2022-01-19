using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour, ITeamComponent, IWeapon
{
    public float Damage;
    public float Firerate;
    public DamageMatrix.Damage DamageType;
    public float Speed;
    public int Amount = 1;

    public float BurstReloadTime;
    public int BurstAmmo = 1;

    private int _currentBurstAmmo;
    private bool _chambered;

    public ParticleSystem FireParticle;

    public GameObject ProjectilePrefab;
    public Transform Muzzle;
    public float Inaccuracy;

    private IObjectPool _pool;
    private LayerMask _hitLayerMask;

    float IWeapon.Damage => Damage * Amount;
    float IWeapon.Firerate => Firerate;
    float IWeapon.Speed => Speed;

    DamageMatrix.Damage IWeapon.DamageType => DamageType;

    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;

    private void Start()
    {
        _pool = ObjectPool.GetPool(ProjectilePrefab);
        _currentBurstAmmo = BurstAmmo;
        _chambered = true;
    }

    public virtual bool TryFire(ITarget intendedTarget)
    {
        if (CanFire())
        {
            Fire(intendedTarget);
            _chambered = false;
            _currentBurstAmmo--;
            Invoke("Rechamber", 1f / Firerate);

            if (_currentBurstAmmo == 0)
            {
                Invoke("Reload", BurstReloadTime);
            }

            return true;
        }
        return false;
    }

    private void Fire(ITarget intendedTarget)
    {
        if (FireParticle)
        {
            FireParticle.Play();
        }

        for (int i = 0; i < Amount; i++)
        {
            Quaternion rotation = Muzzle.transform.rotation * Quaternion.Euler (UnityEngine.Random.Range(-Inaccuracy, Inaccuracy), UnityEngine.Random.Range(-Inaccuracy, Inaccuracy), 0f);
            GameObject proj = _pool.GetObject(Muzzle.transform.position, rotation);

            proj.transform.position = Muzzle.transform.position;
            proj.transform.rotation = Muzzle.transform.rotation;

            Projectile projectile = proj.GetComponent<Projectile>();
            projectile.HitLayerMask = _hitLayerMask;
            projectile.Target = intendedTarget;

            projectile.Damage = Damage;
            projectile.DamageType = DamageType;
            projectile.Speed = Speed;

            projectile.Fire(rotation * Vector3.forward);

            projectile.OnHit += Projectile_OnHit;
            projectile.OnKill += Projectile_OnKill;
            projectile.OnEnd += Projectile_OnEnd;

            OnProjectile?.Invoke(this, projectile);
        }

        OnFire?.Invoke(this);
    }

    private void Projectile_OnEnd(Projectile projectile)
    {
        projectile.OnHit -= Projectile_OnHit;
        projectile.OnKill -= Projectile_OnKill;
        projectile.OnEnd -= Projectile_OnEnd;
    }

    private void Projectile_OnKill(Projectile projectile, IDamagable damagable)
    {
        OnKill?.Invoke(this, projectile, damagable);
    }

    private void Projectile_OnHit(Projectile projectile, Collider col, Vector3 point, Vector3 direction)
    {
        OnHit?.Invoke(this, projectile, col, point, direction);
    }

    private void OnDestroy()
    {
        ObjectPool.FreePool(_pool as ObjectPool);
    }

    public virtual float GetDPS()
    {
        float shotDamage = Damage * Amount;
        return Mathf.Min(shotDamage * Firerate, shotDamage * (BurstAmmo / BurstReloadTime));
    }

    public virtual bool CanFire()
    {
        return _chambered && _currentBurstAmmo > 0;
    }

    private void Rechamber()
    {
        _chambered = true;
    }

    private void Reload()
    {
        _currentBurstAmmo = BurstAmmo;
    }

    public void SetTeam(TeamInfo faction)
    {
        _hitLayerMask = faction.GetOtherLayerMasks();
    }
}
