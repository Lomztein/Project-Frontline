using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commander : MonoBehaviour, ITeamComponent
{
    public string Name;

    public int Credits;
    private float _incomingCredits;

    public TeamInfo TeamInfo;

    public bool CanBuild => Fortress != null;
    public bool Eliminated => _ded;


    public Transform Fortress;
    private List<Unit> _alivePlaced = new List<Unit>();

    private bool _ded = false;

    public UnitSource UnitSource;

    public event Action<Commander, Unit> OnPlacedUnitDeath;
    public event Action<Commander, UnitFactory, Unit> OnUnitSpawned;
    public event Action<Commander> OnFortressDestroyed;
    public event Action<Commander> OnEliminated;

    protected virtual void Awake()
    {
        gameObject.name = Name;
        AssignCommander(gameObject);
    }

    private void Start()
    {
        Team.GetTeam(TeamInfo).AddCommander(this);
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
        go.GetComponent<Health>().OnDeath += OnUnitDeath;

        void OnUnitDeath ()
        {
            u.GetComponent<Health>().OnDeath -= OnUnitDeath;
            _alivePlaced.Remove(placed);
            OnPlacedUnitDeath?.Invoke(this, u);
            if (_alivePlaced.Count == 0)
            {
                OnEliminated?.Invoke(this);
            }
        }


        return go;
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

            return true;
        }
        return false;
    }

    private void Factory_OnUnitSpawned(UnitFactory arg1, GameObject arg2)
    {
        Unit unit = arg2.GetComponent<Unit>();
        unit.OnKill += Unit_OnKill;
        OnUnitSpawned?.Invoke(this, arg1, unit);
    }

    private void Unit_OnKill(Unit arg1, IWeapon arg2, Projectile arg3, IDamagable arg4)
    {
        if (arg4 is Component component)
        {
            var killedUnit = component.GetComponentInParent<Unit>();
            Earn(killedUnit.Info.Value);
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
    }

    public void SetTeam(TeamInfo faction)
    {
        TeamInfo = faction;
    }
}
