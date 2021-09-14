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

    DamageMatrix.Damage IWeapon.DamageType => DamageType;

    public event Action OnFire;

    private void Start()
    {
        _pool = ObjectPool.GetPool(ProjectilePrefab);
        _currentBurstAmmo = BurstAmmo;
        _chambered = true;
    }

    public virtual void TryFire(ITarget intendedTarget)
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
        }
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
        }

        OnFire?.Invoke();
    }

    private void OnDestroy()
    {
        ObjectPool.FreePool(_pool as ObjectPool);
    }

    public float GetDPS() => Firerate * Damage * Amount;

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
