using UnityEngine;

public class Weapon : MonoBehaviour, IFactionComponent
{
    public float Firerate;
    public float BurstReloadTime;
    public int BurstAmmo;

    private int _currentBurstAmmo;
    private bool _chambered;

    public ParticleSystem FireParticle;

    public GameObject ProjectilePrefab;
    public Transform Muzzle;
    public float Inaccuracy;

    public LayerMask LayerMask;

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
        }
        if (_currentBurstAmmo < 1 && !IsInvoking())
        {
            Invoke("Reload", BurstReloadTime);
        }
    }

    private void Fire()
    {
        FireParticle.Play();
        GameObject proj = Instantiate(ProjectilePrefab, Muzzle.transform.position, Muzzle.transform.rotation);
        Projectile projectile = proj.GetComponent<Projectile>();

        float rad = Inaccuracy * Mathf.Deg2Rad;
        Vector3 angled = Muzzle.forward + Muzzle.rotation * (Vector3.right * Mathf.Sin(Random.Range(-rad, rad)) + Vector3.up * Mathf.Sin(Random.Range(-rad, rad)));

        projectile.Fire(angled);
    }

    public bool CanFire ()
    {
        return _chambered && _currentBurstAmmo > 0;
    }

    private void Rechamber ()
    {
        _chambered = true;
    }

    private void Reload ()
    {
        _currentBurstAmmo = BurstAmmo;
    }

    public void SetFaction(Faction faction)
    {
        LayerMask = faction.GetOtherLayerMasks();
    }
}
