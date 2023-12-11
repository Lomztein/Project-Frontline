using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using Util;

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

    public OffcenterBounds LocalBaseBounds { get; private set; }

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

    [Header("Frontline")]
    public Commander Target;
    public Frontline Frontline;

    public float DefenseMargin = 50;
    public float DefenseThreshold = 100;

    public float OffenseMargin = 100;
    public float OffenseThreshold = 200;

    public float BaseMargin = 50;
    public Vector3 BaseSize => LocalBaseBounds.Size;
    public Vector3 BaseSizeWithMargin => LocalBaseBounds.Size + new Vector3(BaseMargin, BaseMargin, BaseMargin);

    public Bounds DefenseVolumeLocalBounds
        => new Bounds(Vector3.forward * (LocalBaseBounds.UpperExtends.z + DefenseMargin + (DefenseThreshold - DefenseMargin) / 2), new Vector3(BaseSizeWithMargin.x, 1f, DefenseThreshold - DefenseMargin)); // mmmhm nice and maintainable
    public Bounds OffenseVolumeLocalBounds
        => new Bounds(Vector3.forward * (Target.LocalBaseBounds.UpperExtends.z + OffenseMargin + (OffenseThreshold - OffenseMargin) / 2), new Vector3(Target.BaseSizeWithMargin.x, 1f, OffenseThreshold - OffenseMargin));

    public float OffenseFactor { get; private set; }
    public float DefenseFactor { get; private set; }

    private float[] _earningsAverageWindow = new float[10];
    private int _earningsAverageIndex;
    private float _currentIndexEarnings;

    private List<IUnitCostModifier> _globalUnitCostModifiers = new List<IUnitCostModifier>();
    private List<IUnitPurchasePredicate> _globalUnitPurchasePredicate = new List<IUnitPurchasePredicate>();

    public void AddGlobalUnitCostModifier(IUnitCostModifier modifier) => _globalUnitCostModifiers.Add(modifier);
    public void RemoveGLobalUnitCostModifier(IUnitCostModifier modifier) => _globalUnitCostModifiers.Remove(modifier);

    public void AddGlobalUnitPurchasePredicate(IUnitCostModifier modifier) => _globalUnitCostModifiers.Add(modifier);
    public void RemoveGlobalUnitPurchasePredicate(IUnitCostModifier modifier) => _globalUnitCostModifiers.Remove(modifier);

    public IEnumerable<IUnitCostModifier> GlobalUnitCostModifiers => _globalUnitCostModifiers;
    public IEnumerable<IUnitPurchasePredicate> GlobalUnitPurchasePredicates => _globalUnitPurchasePredicate;

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

    public GameObject GetProducedUnitIfFactory(GameObject unit)
    {
        if (unit.TryGetComponent(out UnitFactory factory))
            return factory.UnitPrefab;
        return unit;
    }

    protected virtual void Start()
    {
        Team.GetTeam(TeamInfo).AddCommander(this);
        InvokeRepeating(nameof(MoveNextAverageEarnings), 1f, 1f);
        UpdateBaseBounds();
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
            if (!Target)
            {
                Target = FindTarget();
            }

            if (Target)
            {
                DefenseFactor = CalcFrontlineState(transform, LocalBaseBounds, Frontline.Position, DefenseMargin, DefenseThreshold);
                OffenseFactor = CalcFrontlineState(Target.transform, Target.LocalBaseBounds, Frontline.Position, OffenseMargin, OffenseThreshold);
                if (Target.Eliminated) Target = null;
            }
        }
    }

    private static float CalcFrontlineState(Transform transform, OffcenterBounds localBaseBounds, Vector3 frontlinePosition, float margin, float threshold)
    {
        Vector3 extends = localBaseBounds.UpperExtends;
        threshold += extends.z;
        margin += extends.z;

        float frontlineDistFromBase = VectorUtils.DifferenceAlongDirection(Vector3.forward, transform.InverseTransformPoint(frontlinePosition), Vector3.zero);
        return Mathf.InverseLerp(threshold, margin, frontlineDistFromBase);
    }

    private Commander FindTarget ()
    {
        return UnityUtils.FindBest(Team.GetOtherTeams(TeamInfo).
            SelectMany(x => x.GetCommanders()).
            Where(x => !x.Eliminated), x => CalcTargetScore(x.transform.position));
    }

    private float CalcTargetScore(Vector3 target)
    {
        Vector3 localPos = transform.InverseTransformPoint(target);
        float vecAngle = Mathf.Atan2(localPos.x, localPos.z) * Mathf.Rad2Deg;
        return -Mathf.Abs(vecAngle - 0.1f); // Prioritize most directly ahead with a slight clockwise bias to break stalemates.
    }

    private void UpdateBaseBounds()
    {
        Matrix4x4 local = transform.worldToLocalMatrix;
        OffcenterBounds bounds = new OffcenterBounds(Vector3.zero, Vector3.zero, Vector3.zero);
        var structures = Enumerable.Concat(AlivePlaced, GetComponentInChildren<Unit>().SingleObjectAsEnumerable());
        foreach (var placed in structures)
        {
            if (placed)
            {
                Bounds objBounds = UnityUtils.ComputeObjectColliderBounds(placed.gameObject); // Objects world space bounds.
                objBounds.center = local.MultiplyPoint(objBounds.center);
                objBounds.size = Quaternion.Inverse(local.rotation) * objBounds.size;
                bounds.Encapsulate(objBounds);
            }
        }
        LocalBaseBounds = bounds;
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
            UpdateBaseBounds();
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
        if (TrySpend(unit.GetCost(this)) && CanBuild)
        {
            GameObject placedUnit = PlaceUnit(GeneratePrefab(unitPrefab), position, rotation);
            UnitFactory factory = placedUnit.GetComponent<UnitFactory>();
            if (factory)
            {
                factory.OnUnitSpawned += Factory_OnUnitSpawned;
                // TODO: Clean up.
            }
            OnUnitPlaced?.Invoke(this, placedUnit.GetComponent<Unit>());
            UpdateBaseBounds();
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
        if (IsValidFrontlineRegister(obj.transform.position))
        {
            Frontline.Register(obj.transform.position);
        }
    }

    private bool IsValidFrontlineRegister(Vector3 pos)
    {
        float dist = Vector3.SqrMagnitude(pos - transform.position);
        float targetDist = Vector3.SqrMagnitude(Target.transform.position - transform.position);
        return dist < targetDist;
    }

    private void Unit_OnKill(Unit arg1, IWeapon arg2, Projectile arg3, IDamagable arg4)
    {
        if (arg4 is Component component)
        {
            var killedUnit = component.GetComponentInParent<Unit>();
            if (killedUnit)
            {
                Earn(killedUnit.Info.Value);
                if (IsValidFrontlineRegister(component.transform.position))
                {
                    Frontline.Register(component.transform.position);
                }
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

    public OverlapUtils.OverlapShape GetUnitPlacementOverlapShape (GameObject unit) // This should not be unique to Commander.
    {
        GameObject factory = GetUnitFactoryPrefab(unit);
        if (factory)
        {
            return OverlapUtils.CreateFromCollidersIn(factory, OverlapShapeFilter);
        }
        return OverlapUtils.CreateFromCollidersIn(unit, OverlapShapeFilter);
    }

    private bool OverlapShapeFilter(Collider col)
    {
        return !col.CompareTag("Shield");
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

    public bool CanPlace(Vector3 position, Quaternion rotation, OverlapUtils.OverlapShape unitOverlapGroup)
    {
        LayerMask terrainLayer = LayerMask.NameToLayer("Terrain");
        Collider[] colliders = unitOverlapGroup.Overlap(position, rotation, ~terrainLayer);
        return !colliders.Any(x => x.CompareTag("StructureUnit")) && MatchSettings.Current.MapInfo.Contains(position);
    }

    private void OnDrawGizmos()
    {
        if (Frontline != null && Target)
        {
            Bounds bounds = LocalBaseBounds.ToBounds();
            Gizmos.DrawSphere(Frontline.Position, 1f);
            Gizmos.DrawRay(Frontline.Position, Frontline.Change);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawRay(transform.localPosition, Vector3.forward * LocalBaseBounds.UpperExtends.z);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(DefenseVolumeLocalBounds.center, DefenseVolumeLocalBounds.size);
            Gizmos.color = Color.red;
            Gizmos.matrix = Target.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(OffenseVolumeLocalBounds.center + Vector3.up, OffenseVolumeLocalBounds.size);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.transform.position + Vector3.up * 10);
        }
    }

    public int GetCost(GameObject prefab)
        => GetCost(prefab.GetComponent<Unit>());
    public int GetCost(Unit unit)
        => PurchasingUtils.GetCost(unit, unit.BaseCost, Enumerable.Concat(GlobalUnitCostModifiers, unit.GetCostModifiers()), this);
    public bool CanAfford(GameObject prefab)
        => Credits >= GetCost(prefab);
    public bool CanPurchase(GameObject unit)
        => CanPurchase(unit.GetComponent<Unit>());
    public bool CanPurchase(Unit unit)
        => PurchasingUtils.CanPurchase(unit, Enumerable.Concat(GlobalUnitPurchasePredicates, unit.GetPurchasePredicates()), this);
    public bool CanAffordAndPurchase(GameObject prefab)
        => CanAfford(prefab) && CanPurchase(prefab);
    public string GetCanAffordDescription(GameObject prefab)
        => GetCanAffordDescription(prefab.GetComponent<Unit>());
    public string GetCanAffordDescription(Unit unit)
        => PurchasingUtils.GetCostModifierDesription(unit, unit.BaseCost, Enumerable.Concat(GlobalUnitCostModifiers, unit.GetCostModifiers()), this);
    public string GetCanPurchaseDescription(GameObject unit)
        => GetCanPurchaseDescription(unit.GetComponent<Unit>());
    public string GetCanPurchaseDescription(Unit unit)
        => PurchasingUtils.GetCanPurchaseDesription(unit, Enumerable.Concat(GlobalUnitPurchasePredicates, unit.GetPurchasePredicates()), this);
    public string GetCanAffordAndPurchaseDescription(GameObject prefab)
        => (GetCanAffordDescription(prefab) + "\n" + GetCanPurchaseDescription(prefab)).Trim();
}
