using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour, IPurchasable, ICommanderComponent, ITeamComponent
{
    public const float ENGAGE_TIME = 10f;
    public UnitInfo Info;

    public string Name => Info.Name;
    public string Description => Info.Description;
    public Sprite Sprite => null;
    public Health Health;

    [SerializeField] private GameObject[] _weapons;
    private List<IWeapon> _weaponCache;
    public IEnumerable<IWeapon> GetWeapons() => GetWeaponCache(ref _weaponCache);

    public event Action<Unit, IWeapon, Projectile, IDamagable> OnKill;

    private float _lastEngageTime;

    public bool IsEngaged => _lastEngageTime + ENGAGE_TIME > Time.time;

    public int BaseCost => Info.Cost;

    private float _effeciencyScore;
    private float _effeciencyTarget;
    public float Effeciency => _effeciencyScore / _effeciencyTarget;

    public int GetCost(Commander commander) => commander.GetCost(gameObject);

    private Dictionary<string, float> _stats = new Dictionary<string, float>();

    [Header("Parts")] // UnitParts are specifically designated parts of the unit that might be of interest to others.
    public UnitPart[] Parts;
    public WeakpointUnitPart[] Weakpoints;

    public IEnumerable<T> GetParts<T>() where T : UnitPart
        => Parts.Where(x => x != null).Select(x => x as T).Where(x => x != null);

    public IEnumerable<IUnitCostModifier> GetCostModifiers()
        => GetComponents<IUnitCostModifier>();
    public IEnumerable<IUnitPurchasePredicate> GetPurchasePredicates()
        => GetComponents<IUnitPurchasePredicate>();

    public Commander Commander { get; private set; }
    public TeamInfo TeamInfo { get; private set; }

    private void CheckBattlefieldBounds()
    {
        // Kill the unit if they go way too far out of bounds.
        if (!MatchSettings.Current.MapInfo.Contains(transform.position / 2f))
        {
            Health.TakeDamage(new DamageInfo(Health.MaxHealth / 3f, DamageModifier.One, transform.position, transform.forward, this, Health));
        }
    }

    public void AddWeapon(IWeapon weapon)
    {
        _weaponCache.Add(weapon);
        weapon.OnKill += Weapon_OnKill;
        weapon.OnHit += Weapon_OnHit;
    }

    public void RemoveWeapon(IWeapon weapon)
    {
        _weaponCache.Remove(weapon);
        weapon.OnKill -= Weapon_OnKill;
        weapon.OnHit -= Weapon_OnHit;
    }

    private void Awake()
    {
        Weakpoints = GetParts<WeakpointUnitPart>().ToArray();
        GetWeaponCache(ref _weaponCache);

        Health = GetComponent<Health>();
        InvokeRepeating(nameof(CheckBattlefieldBounds), 10f, 10f);
    }

    private IEnumerable<IWeapon> GetWeaponCache(ref List<IWeapon> cache)
    {
        if (cache == null)
        {
            cache = new List<IWeapon>();
            foreach (GameObject weaponObj in _weapons)
            {
                if (weaponObj.TryGetComponent<IWeapon>(out var weapon))
                {
                    AddWeapon(weapon);
                }
            }
        }
        return cache;
    }

    public float AddToEffeciencyScore(float score) 
        => _effeciencyScore += score;

    public float AddToEffeciencyTarget(float score)
        => _effeciencyTarget += score;

    private void Start()
    {
        Health.OnDamageTaken += Health_OnDamageTaken;
    }

    private void Health_OnDamageTaken(Health arg1, DamageInfo info)
    {
        _lastEngageTime = Time.time;
    }

    public Dictionary<string, float> GetStats()
        => new Dictionary<string, float>(_stats);

    public float SetStat(string stat, float value)
    {
        _stats[stat] = value;
        return _stats[stat];
    }

    public float? GetStat(string stat)
    {
        if (_stats.TryGetValue(stat, out var value)) return value;
        return null;
    }

    public float ChangeStat(string stat, float value)
    {
        float val = GetStat(stat) ?? 0;
        return SetStat(stat, val + value);
    }

    private void Weapon_OnHit(IWeapon arg1, Projectile arg2, Collider arg3, Vector3 arg4, Vector3 arg5)
    {
        _lastEngageTime = Time.time;
    }

    private void Weapon_OnKill(IWeapon arg1, Projectile arg2, IDamagable arg3)
    {
        OnKill?.Invoke(this, arg1, arg2, arg3);
        ChangeStat("Kills", 1);
    }

    public WeaponInfo[] GetWeaponInfo()
    {
        return GetComponentsInChildren<WeaponInfo>();
    }

    public void AssignCommander(Commander commander)
    {
        Commander = commander;
    }

    public void SetTeam(TeamInfo team)
    {
        TeamInfo = team;
    }
}
