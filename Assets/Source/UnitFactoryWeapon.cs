using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitFactoryWeapon : MonoBehaviour, ITeamComponent, IWeapon
{
    public GameObject[] UnitPrefabs;
    public enum UnitSelectionBehaviour { Random }
    public UnitSelectionBehaviour SelectionBehaviour;

    public float Cooldown;
    private float _currentCooldown;
    public float PlaceDelay;
    private bool _canPlace = true;

    private int _currentHolding;
    public int MaxHolding = 1;

    public int MaxSimultanious;
    private int _currentSimultanious;

    public float SpawnRange;

    private TeamInfo _team;

    private IEnumerable<IWeapon> GetUnitPrefabWeapons() => UnitPrefabs.SelectMany(x => x.GetComponentsInChildren<IWeapon>());

    public float Damage => GetUnitPrefabWeapons().Sum(x => x.Damage);
    public float Firerate => 1 / Cooldown;
    public float Speed => 0f;

    public DamageMatrix.Damage DamageType => GetUnitPrefabWeapons().First().DamageType;

    public event Action<UnitFactoryWeapon, GameObject> OnUnitSpawned;

    public event Action<IWeapon> OnFire;
    public event Action<IWeapon, Projectile> OnProjectile;
    public event Action<IWeapon, Projectile, Collider, Vector3, Vector3> OnHit;
    public event Action<IWeapon, Projectile, IDamagable> OnKill;

    private GameObject SelectUnitPrefab ()
    {
        switch (SelectionBehaviour)
        {
            case UnitSelectionBehaviour.Random:
                return UnitPrefabs[UnityEngine.Random.Range(0, UnitPrefabs.Length)];
            default:
                return null;
        }
    }

    private void FixedUpdate()
    {
        if (_currentHolding < MaxHolding)
        {
            _currentCooldown -= Time.fixedDeltaTime;
            if (_currentCooldown <= 0f)
            {
                _currentCooldown = Cooldown;
                _currentHolding++;
            }
        }
        else
        {
            _currentCooldown = Cooldown;
        }
    }

    private GameObject Spawn()
    {
        Vector3 pos = GetLocalRandomSpawnPosition() + transform.position;
        GameObject go = _team.Instantiate(SelectUnitPrefab(), pos, transform.rotation);
        OnUnitSpawned?.Invoke(this, go);
        return go;
    }

    private Vector3 GetLocalRandomSpawnPosition()
    {
        Vector3 unitSphere = UnityEngine.Random.insideUnitSphere * SpawnRange;
        return new Vector3(unitSphere.x, 0f, unitSphere.z);
    }

    public void SetTeam(TeamInfo faction)
    {
        _team = faction;
    }

    public bool CanFire()
    {
        return _canPlace && _currentSimultanious < MaxSimultanious && _currentHolding > 0;
    }

    public bool TryFire(ITarget intendedTarget)
    {
        if (CanFire())
        {
            Debug.Log("Spawn!");
            GameObject go = Spawn();
            _currentSimultanious++;

            go.GetComponent<AIController>().ForceTarget(intendedTarget);
            go.GetComponent<Health>().OnDeath += () => _currentSimultanious--;

            _canPlace = false;
            Invoke(nameof(CanPlaceAgain), PlaceDelay);
            return true;
        }
        return false;
    }

    private void CanPlaceAgain()
    {
        _canPlace = true;
    }

    public float GetDPS()
    {
        return GetUnitPrefabWeapons().Sum(x => x.GetDPS()) / UnitPrefabs.Length * MaxSimultanious; // It's a very, very rough estimate, but might just work for the AI.
    }
}
