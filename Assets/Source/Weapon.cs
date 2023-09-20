using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour, ITeamComponent, IWeapon
{
    public float Damage;
    public float Firerate;
    public DamageModifier Modifier;
    public float Speed;
    public float Range;
    public int Amount = 1;

    public float BurstReloadTime;
    public int BurstAmmo = 1;

    private int _currentBurstAmmo;
    public bool IsChambered = true;
    private bool _isChambering = false;

    public ParticleSystem FireParticle;
    public LightFlash LightFlash;
    public AudioClip FireAudioClip;
    private AudioSource _audioSource;
    public Effect FireEffect;

    public GameObject ProjectilePrefab;
    public Transform Muzzle;
    public float Inaccuracy;
    public float SpeedVariance = 0f;

    private IObjectPool _pool;
    private LayerMask _hitLayerMask;
    private TeamInfo _team;

    float IWeapon.Damage => Damage * Amount;
    float IWeapon.Firerate => Firerate;
    float IWeapon.Speed => Speed;
    DamageModifier IWeapon.Modifier => Modifier;

    int IWeapon.Ammo => BurstAmmo == 1 ? 
        (CanFire() ? 1 : 0)
        : _currentBurstAmmo;
    int IWeapon.MaxAmmo => BurstAmmo;


    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDoDamage;
    public event Action<IWeapon, Projectile, IDamagable, DamageInfo> OnDamageDone;

    private void Start()
    {
        if (!IsChambered) Rechamber(1f / Firerate);
        _pool = ObjectPool.GetPool(ProjectilePrefab);
        _audioSource = GetComponent<AudioSource>();
        _currentBurstAmmo = BurstAmmo;
    }

    public virtual bool TryFire(ITarget intendedTarget)
    {
        if (CanFire())
        {
            Fire(intendedTarget);
            IsChambered = false;
            _currentBurstAmmo--;
            StartCoroutine(Rechamber((1f / Firerate)));

            if (_currentBurstAmmo == 0)
            {
                Invoke("Reload", BurstReloadTime);
            }

            return true;
        }if (!IsChambered && !_isChambering)
        {
            StartCoroutine(Rechamber((1f / Firerate)));
        }
        return false;
    }

    private void Fire(ITarget intendedTarget)
    {
        if (FireParticle)
        {
            FireParticle.Play();
        }
        if (LightFlash)
        {
            LightFlash.Play();
        }
        if (FireEffect)
        {
            FireEffect.Play();
        }
        if (_audioSource && FireAudioClip)
        {
            _audioSource.PlayOneShot(FireAudioClip);
        }

        for (int i = 0; i < Amount; i++)
        {
            Vector2 deviance = GetProjectileInaccuracy();
            Quaternion rotation = Muzzle.transform.rotation * Quaternion.Euler (deviance.x, deviance.y, 0f);
            GameObject proj = _pool.GetObject(Muzzle.transform.position, rotation);

            if (_team)
                _team.ApplyTeam(proj);
            proj.transform.position = Muzzle.transform.position;
            proj.transform.rotation = Muzzle.transform.rotation;

            Projectile projectile = proj.GetComponent<Projectile>();
            projectile.HitLayerMask = _hitLayerMask;
            projectile.Target = intendedTarget;

            projectile.Damage = Damage;
            projectile.Modifier = Modifier;
            projectile.Speed = GetProjectileSpeed();
            projectile.Life = Range / projectile.Speed;

            projectile.OnHit += Projectile_OnHit;
            projectile.OnKill += Projectile_OnKill;
            projectile.OnEnd += Projectile_OnEnd;
            projectile.OnDoDamage += Projectile_OnDoDamage;
            projectile.OnDamageDone += Projectile_OnDamageDone;

            projectile.Fire(rotation * Vector3.forward);
            OnProjectile?.Invoke(this, projectile);
        }

        OnFire?.Invoke(this);
    }

    private void Projectile_OnDamageDone(Projectile arg1, IDamagable arg2, DamageInfo arg3)
    {
        OnDamageDone?.Invoke(this, arg1, arg2, arg3);
    }

    private void Projectile_OnDoDamage(Projectile arg1, IDamagable arg2, DamageInfo arg3)
    {
        OnDoDamage?.Invoke(this, arg1, arg2, arg3);
    }

    private Vector2 GetProjectileInaccuracy()
    {
        return UnityEngine.Random.insideUnitCircle * Inaccuracy;
    }

    private float GetProjectileSpeed()
    {
        return Speed * UnityEngine.Random.Range(1f - SpeedVariance, 1f + SpeedVariance);
    }

    private void Projectile_OnEnd(Projectile projectile)
    {
        projectile.OnHit -= Projectile_OnHit;
        projectile.OnKill -= Projectile_OnKill;
        projectile.OnEnd -= Projectile_OnEnd;
        projectile.OnDoDamage -= Projectile_OnDoDamage;
        projectile.OnDamageDone -= Projectile_OnDamageDone;
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
        if (BurstReloadTime > 0)
        {
            float burstDamage = shotDamage * BurstAmmo;
            float burstTime = (BurstAmmo - 1) * (1f / Firerate);
            return burstDamage / (BurstReloadTime + burstTime);
        }
        return shotDamage * Firerate;
    }

    public virtual bool CanFire()
    {
        return IsChambered && _currentBurstAmmo > 0;
    }

    private IEnumerator Rechamber (float time)
    {
        _isChambering = true;
        yield return new WaitForSeconds(time);
        _isChambering = false;
        IsChambered = true;
    }

    private void Reload()
    {
        _currentBurstAmmo = BurstAmmo;
    }

    public void SetTeam(TeamInfo team)
    {
        _team = team;
        SetHitLayerMask(team.GetOtherLayerMasks());
    }

    public void SetHitLayerMask(LayerMask mask)
        => _hitLayerMask = mask;
}
