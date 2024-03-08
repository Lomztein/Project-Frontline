using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour, IPurchasable, ICommanderComponent, ITeamComponent
{
    public const float ENGAGE_TIME = 10f;
    public UnitInfo Info;

    public string Name => Info.Name;
    public string Description => Info.ShortDescription;
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
    private bool _initialCommanderSet = false;
    public Commander InitialCommander { get; private set; }

    public TeamInfo TeamInfo { get; private set; }
    private bool _initialTeamSet = false;
    public TeamInfo InitialTeamInfo { get; private set; }

    public void AddWeapon(IWeapon weapon)
    {
        _weaponCache.Add(weapon);
        weapon.OnKill += Weapon_OnKill;
        weapon.OnHit += Weapon_OnHit;
        weapon.OnDamageDone += Weapon_OnDamageDone;
        AddStat("Kills", 0);
        AddStat("Damage done", 0);
    }


    public void RemoveWeapon(IWeapon weapon)
    {
        _weaponCache.Remove(weapon);
        weapon.OnKill -= Weapon_OnKill;
        weapon.OnHit -= Weapon_OnHit;
        weapon.OnDamageDone -= Weapon_OnDamageDone;
    }

    private void Awake()
    {
        Weakpoints = GetParts<WeakpointUnitPart>().ToArray();
        GetWeaponCache(ref _weaponCache);

        Health = GetComponent<Health>();
        AddStat("Damage taken", 0f);
        AddStat("Damage healed", 0f);

        if (TryGetComponent(out Rigidbody body))
        {
            Destroy(body);
        }
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
        Health.OnHeal += Health_OnHeal;
    }

    private void Health_OnHeal(Health arg1, DamageInfo arg2)
    {
        ChangeStat("Damage healed", arg2.DamageDone);
    }

    private void Health_OnDamageTaken(Health arg1, DamageInfo info)
    {
        _lastEngageTime = Time.time;
        ChangeStat("Damage taken", info.DamageDone);
    }


    private void Weapon_OnDamageDone(IWeapon arg1, Projectile arg2, IDamagable arg3, DamageInfo arg4)
    {
        ChangeStat("Damage done", arg4.DamageDone);
    }

    public Dictionary<string, float> GetStats()
        => new Dictionary<string, float>(_stats);

    public bool AddStat(string name, float initialValue)
    {
        if (_stats.ContainsKey(name))
        {
            return false;
        }
        _stats.Add(name, initialValue);
        return true;
    }

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

    public EquipmentInfo[] GetWeaponInfo()
    {
        return GetComponentsInChildren<EquipmentInfo>();
    }

    public void AssignCommander(Commander commander)
    {
        Commander = commander;
        if (!_initialCommanderSet)
        {
            InitialCommander = commander;
            _initialCommanderSet = true;
        }
    }

    public void SetTeam(TeamInfo team)
    {
        TeamInfo = team;
        if (!_initialTeamSet)
        {
            InitialTeamInfo = team;
            _initialTeamSet = true;
        }
    }
}
