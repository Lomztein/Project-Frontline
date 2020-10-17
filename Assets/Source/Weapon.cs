using System;
using UnityEngine;

public class Weapon : MonoBehaviour, IFactionComponent, IWeapon
{
    public float Firerate;
    public float BurstReloadTime;
    public int BurstAmmo;

    private int _currentBurstAmmo;
    private bool _chambered;

    private bool _isRechambering;
    private bool _isReloading;

    public ParticleSystem FireParticle;

    public GameObject ProjectilePrefab;
    public Transform Muzzle;
    public float Inaccuracy;

    private Faction _faction;

    public event Action OnFire;

    private void Start()
    {
        _currentBurstAmmo = BurstAmmo;
        _chambered = true;
    }

    public void TryFire()
    {
        if (CanFire())
        {
            Fire();
            _chambered = false;
            _currentBurstAmmo--;
            Invoke("Rechamber", 1f / Firerate);

            if (_currentBurstAmmo == 0)
            {
                Invoke("Reload", BurstReloadTime);
            }
        }
    }

    private void Fire()
    {
        FireParticle.Play();
        GameObject proj = Instantiate(ProjectilePrefab, Muzzle.transform.position, Muzzle.transform.rotation);
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.SetFaction(_faction);

        float rad = Inaccuracy * Mathf.Deg2Rad;
        Vector3 angled = Muzzle.forward + Muzzle.rotation * (Vector3.right * Mathf.Sin(UnityEngine.Random.Range(-rad, rad)) + Vector3.up * Mathf.Sin(UnityEngine.Random.Range(-rad, rad)));
        OnFire?.Invoke();

        projectile.Fire(angled);
    }

    public bool CanFire()
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
