using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commander : MonoBehaviour, ITeamComponent
{
    public string Name;
    public float BuildRadius;

    public int Credits;
    private float _incomingCredits;

    public TeamInfo TeamInfo;
    public Faction Faction;
    public Dictionary<GameObject, bool> UnitAvailable;

    public UnitPalette UnitPalette;

    public bool CanBuild => Fortress != null;
    public bool Eliminated => _ded;

    public Transform Fortress;
    private List<Unit> _alivePlaced = new List<Unit>();
    private List<Unit> _aliveProduced = new List<Unit>();

    public IEnumerable<Unit> AlivePlaced => _alivePlaced;
    public IEnumerable<Unit> AliveProduced => _aliveProduced;
    public IEnumerable<Unit> AliveAll => Enumerable.Concat(AlivePlaced, AliveProduced);

    private bool _ded = false;

    public UnitSource UnitSource;

    public event Action<Commander, Unit> OnUnitPlaced;
    public event Action<Commander, UnitFactory, Unit> OnUnitSpawned;

    public event Action<Commander, Unit> OnPlacedUnitDeath;
    public event Action<Commander> OnFortressDestroyed;
    public event Action<Commander> OnEliminated;

    private float _mapCenterDistance;
    public float OffenseFactorMargin = 100;

    public Frontline Frontline;

    public float OffenseFactor => Eliminated ? 0f : Mathf.InverseLerp(_mapCenterDistance - OffenseFactorMargin, _mapCenterDistance + OffenseFactorMargin, Vector3.Distance(Fortress.position, Frontline.Position)) * 2f - 1f;
    public float DefenseFactor => -OffenseFactor;

    public float MaxSiegeTime = 60f;
    public float OffensiveSiegeTime { get; private set; }
    public float DefensiveSiegeTime { get; private set; }

    private float[] _earningsAverageWindow = new float[10];
    private int _earningsAverageIndex;
    private float _currentIndexEarnings;

    public float AverageIncomePerSecond => _earningsAverageWindow.Average();

    protected virtual void Awake()
    {
        gameObject.name = Name;
        Frontline = new Frontline(10, 3);
        AssignCommander(gameObject);
    }

    public bool IsUnitAvailable(GameObject unit)
    {
        if (UnitAvailable.TryGetValue(unit, out bool value))
        {
            return value;
        }
        return true; // Default to available.
    }

    private void MoveNextAverageEarnings ()
    {
        _earningsAverageWindow[_earningsAverageIndex] = _currentIndexEarnings;
        _currentIndexEarnings = 0f;

        _earningsAverageIndex++;
        if (_earningsAverageIndex >= _earningsAverageWindow.Length)
        {
            _earningsAverageIndex = 0;
        }
    }

    private void Start()
    {
        Team.GetTeam(TeamInfo).AddCommander(this);
        InvokeRepeating(nameof(MoveNextAverageEarnings), 1f, 1f);
        _mapCenterDistance = Vector3.Distance(Fortress.position, Vector3.zero);
    }

    private void OnDestroy()
    {
        if (Team.GetTeam(TeamInfo) != null)
            Team.GetTeam(TeamInfo).RemoveCommander(this);
    }

    protected virtual void FixedUpdate ()
    {
        if (_ded == false && !Fortress)
        {
            _ded = true;
            OnFortressDestroyed?.Invoke(this);
        }

        if (!_ded)
        {
            OffensiveSiegeTime = Mathf.Clamp(OffensiveSiegeTime + OffenseFactor * Time.fixedDeltaTime, 0f, MaxSiegeTime);
            DefensiveSiegeTime = Mathf.Clamp(DefensiveSiegeTime + DefenseFactor * Time.fixedDeltaTime, 0f, MaxSiegeTime);
        }
    }

    public Unit[] GetPlacedUnits() => _alivePlaced.ToArray();

    public GameObject GeneratePrefab (GameObject unitPrefab)
    {
        ProductionInfo info = unitPrefab.GetComponent<ProductionInfo>();
        if (info)
        {
            UnitFactory factory = info.FactoryPrefab.GetComponent<UnitFactory>();
            factory.UnitPrefab = unitPrefab;
            factory.SpawnDelay = info.ProductionTime;
            return factory.gameObject;
        }
        else
        {
            return unitPrefab;
        }
    }

    protected GameObject PlaceUnit (GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Unit u = prefab.GetComponent<Unit>();
        GameObject go = TeamInfo.Instantiate(prefab, position, rotation);
        Unit placed = go.GetComponent<Unit>();
        AssignCommander(go);
        _alivePlaced.Add(placed);
        go.GetComponent<Health>().OnDeath += PlacedUnitDeath;
        placed.OnKill += Unit_OnKill;

        void PlacedUnitDeath (Health health)
        {
            u.GetComponent<Health>().OnDeath -= PlacedUnitDeath;
            _alivePlaced.Remove(placed);
            OnPlacedUnitDeath?.Invoke(this, u);
            if (_alivePlaced.Count == 0)
            {
                OnEliminated?.Invoke(this);
            }
        }


        return go;
    }

    public bool IsNearAnyPlaced (Vector3 position)
    {
        var toCheck = Enumerable.Concat(Fortress.GetComponent<Unit>().SingleObjectAsEnumerable(), AlivePlaced);
        return toCheck.Any(x => Vector3.Distance(x.transform.position, position) < BuildRadius);
    }

    public bool TryPurchaseAndPlaceUnit(GameObject unitPrefab, Vector3 position, Quaternion rotation)
    {
        Unit unit = unitPrefab.GetComponent<Unit>();
        if (TrySpend(unit.Cost) && CanBuild)
        {
            GameObject placedUnit = PlaceUnit(GeneratePrefab(unitPrefab), position, rotation);
            UnitFactory factory = placedUnit.GetComponent<UnitFactory>();
            if (factory)
            {
                factory.OnUnitSpawned += Factory_OnUnitSpawned;
                // TODO: Clean up.
            }
            OnUnitPlaced?.Invoke(this, placedUnit.GetComponent<Unit>());
            return true;
        }
        return false;
    }

    private void Factory_OnUnitSpawned(UnitFactory arg1, GameObject arg2)
    {
        Unit unit = arg2.GetComponent<Unit>();
        unit.OnKill += Unit_OnKill;
        OnUnitSpawned?.Invoke(this, arg1, unit);
        _aliveProduced.Add(unit);
        unit.GetComponent<Health>().OnDeath += OnProducedUnitDeath;
        AssignCommander(arg2);
    }

    private void OnProducedUnitDeath(Health obj)
    {
        _aliveProduced.Remove(obj.GetComponent<Unit>());
        Frontline.Register(obj.transform.position);
    }

    private void Unit_OnKill(Unit arg1, IWeapon arg2, Projectile arg3, IDamagable arg4)
    {
        if (arg4 is Component component)
        {
            var killedUnit = component.GetComponentInParent<Unit>();
            if (killedUnit)
            {
                Earn(killedUnit.Info.Value);
                Frontline.Register(component.transform.position);
            }
        }
    }

    public GameObject GetUnitFactoryPrefab (GameObject unit) // This should not be unique to Commander.
    {
        ProductionInfo info = unit.GetComponent<ProductionInfo>();
        if (info)
        {
            return info.FactoryPrefab;
        }
        return null;
    }

    public Vector3 GetUnitPlacementCheckSize (GameObject unit) // This should not be unique to Commander.
    {
        GameObject factory = GetUnitFactoryPrefab(unit);
        if (factory)
        {
            // TODO: Create strategy-based size getter thing
            return factory.GetComponentInChildren<BoxCollider>().size;
        }
        return unit.GetComponentInChildren<BoxCollider>().size;
    }

    public bool TrySpend (int credits)
    {
        if (HasCredits(credits))
        {
            Credits -= credits;
            return true;
        }
        return false;
    }

    public bool HasCredits(int credits) => Credits >= credits;

    public void AssignCommander(GameObject obj)
    {
        var components = obj.GetComponentsInChildren<ICommanderComponent>();
        foreach (var component in components)
        {
            component.AssignCommander(this);
        }
    }

    public void Earn (float moneys)
    {
        _incomingCredits += moneys;
        int earned = Mathf.FloorToInt(_incomingCredits);
        _incomingCredits -= earned;
        Credits += earned;
        _currentIndexEarnings += moneys;
    }

    public void SetTeam(TeamInfo faction)
    {
        TeamInfo = faction;
        UnitPalette = UnitPalette.GeneratePalette(Faction.FactionPalette, TeamInfo.TeamPalette);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Frontline.Position, 1f);
        Gizmos.DrawRay(Frontline.Position, Frontline.Change);
    }
}
