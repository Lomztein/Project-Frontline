using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Commander : MonoBehaviour, ITeamComponent
{
    public string Name;

    public int Credits;
    private float _incomingCredits;

    public TeamInfo Team;

    public bool IsEliminated => Fortress == null;
    public Transform Fortress;
    private bool _ded = false;

    public GameObject[] AvailableUnits;

    public event Action<Commander> OnEliminated;

    protected virtual void Awake()
    {
        gameObject.name = Name;
        AssignCommander(gameObject);
    }

    protected virtual void FixedUpdate ()
    {
        if (_ded == false && !Fortress)
        {
            _ded = true;
            OnEliminated?.Invoke(this);
        }
    }

    protected GameObject GeneratePrefab (GameObject unitPrefab)
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

    protected GameObject PlaceUnit (GameObject unit, Vector3 position, Quaternion rotation)
    {
        Unit u = unit.GetComponent<Unit>();
        Credits -= u.Info.Cost;
        GameObject prefab = GeneratePrefab(unit);
        GameObject go = Team.Instantiate(prefab, position, rotation);
        AssignCommander(go);

        PassiveIncome pi = go.GetComponent<PassiveIncome>();
        if (pi)
        {
            pi.IncomePerSecond = u.Info.Value;
        }
        return go;
    }

    public GameObject GetUnitFactoryPrefab (GameObject unit)
    {
        ProductionInfo info = unit.GetComponent<ProductionInfo>();
        if (info)
        {
            return info.FactoryPrefab;
        }
        return null;
    }

    public Vector3 GetUnitPlacementCheckSize (GameObject unit)
    {
        GameObject factory = GetUnitFactoryPrefab(unit);
        if (factory)
        {
            // TODO: Create strategy-based size getter thing
            return factory.GetComponentInChildren<BoxCollider>().size;
        }
        return unit.GetComponentInChildren<BoxCollider>().size;
    }

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
        Team = faction;
    }
}
