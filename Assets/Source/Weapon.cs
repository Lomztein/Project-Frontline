using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour, IFactionComponent, IWeapon
{
    public float Damage;
    public float Firerate;
    public DamageArmorMapping.Damage DamageType;
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

    private Faction _faction;
    private IObjectPool _pool;

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
            GameObject proj = _pool.GetObject();

            proj.transform.position = Muzzle.transform.position;
            proj.transform.rotation = Muzzle.transform.rotation;

            Projectile projectile = proj.GetComponent<Projectile>();
            projectile.SetFaction(_faction);
            projectile.Target = intendedTarget;

            projectile.Damage = Damage;
            projectile.DamageType = DamageType;
            projectile.Speed = Speed;

            float rad = Inaccuracy * Mathf.Deg2Rad;
            Vector3 angled = Muzzle.forward + Muzzle.rotation * (Vector3.right * Mathf.Sin(UnityEngine.Random.Range(-rad, rad)) + Vector3.up * Mathf.Sin(UnityEngine.Random.Range(-rad, rad)));

            projectile.Fire(angled);
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

    public void SetFaction(Faction faction)
    {
        _faction = faction;
    }
}
