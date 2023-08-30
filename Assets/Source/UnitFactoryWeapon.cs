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
    private List<GameObject> _currentSimultanious = new List<GameObject>();
    private List<EngagedTracker> _currentTrackers = new List<EngagedTracker>();
    public bool DestroyUnengagedUnits;
    public float DestroyUnengagedTimeTreshold = 10;

    public float SpawnRange;
    public bool SpawnRelativeToRoot;
    private Transform _root;

    private TeamInfo _team;
    private LayerMask _hitLayerMask;

    private IEnumerable<IWeapon> GetUnitPrefabWeapons() => UnitPrefabs.SelectMany(x => x.GetComponentsInChildren<IWeapon>());

    public float Damage => GetUnitPrefabWeapons().Sum(x => x.Damage);
    public float Firerate => 1 / Cooldown;
    public float Speed => 0f;

    public DamageMatrix.Damage DamageType => GetUnitPrefabWeapons().FirstOrDefault()?.DamageType ?? DamageMatrix.Damage.Gun;

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

    private void Start()
    {
        _root = transform.root;
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
        Vector3 basePos = SpawnRelativeToRoot ? _root.position : transform.position;
        Quaternion baseRot = SpawnRelativeToRoot ? _root.rotation : transform.rotation;
        Vector3 pos = GetLocalRandomSpawnPosition() + basePos;
        GameObject go = _team.Instantiate(SelectUnitPrefab(), pos, baseRot);
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
        _hitLayerMask = faction.GetOtherLayerMasks();
    }

    public bool CanFire()
    {
        return _canPlace && _currentSimultanious.Count < MaxSimultanious && _currentHolding > 0;
    }

    private void DestroyUnengaged ()
    {
        Health toDestroy = null;
        foreach (var tracker in _currentTrackers)
        {
            if (tracker != null && Time.time > tracker.LastAttackTime + DestroyUnengagedTimeTreshold)
            {
                toDestroy = tracker.GetComponent<Health>();
                break;
            }
        }
        if (toDestroy)
        {
            toDestroy.TakeDamage(new DamageInfo(toDestroy.MaxHealth, DamageMatrix.Damage.Heal, transform.position, transform.forward));
        }
    }

    public bool TryFire(ITarget intendedTarget)
    {
        if (DestroyUnengagedUnits && _currentSimultanious.Count == MaxSimultanious && _canPlace && _currentHolding > 0)
        {
            DestroyUnengaged();
        }

        if (CanFire())
        {
            GameObject go = Spawn();
            _currentHolding--;
            _currentSimultanious.Add(go);

            if (go.TryGetComponent(out EngagedTracker tracker))
            {
                _currentTrackers.Add(tracker);

                go.GetComponentInChildren<Health>().OnDeath += (Health health) =>
                {
                    _currentTrackers.Remove(tracker);
                };
            }

            go.GetComponentInChildren<Health>().OnDeath += (Health health) =>
            {
                _currentSimultanious.Remove(go);
            };

            if (go.TryGetComponent(out AIController controller)) {
                controller.ForceTarget(intendedTarget);
                foreach (var weapon in controller.Weapons)
                {
                    weapon.SetHitLayerMask(_hitLayerMask);
                }
            }

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
        return GetUnitPrefabWeapons().Sum(x => x.GetDPSOrOverride()) / UnitPrefabs.Length * MaxSimultanious; // It's a very, very rough estimate, but might just work for the AI.
    }

    public void SetHitLayerMask(LayerMask mask)
    {
        _hitLayerMask = mask;
    }
}
