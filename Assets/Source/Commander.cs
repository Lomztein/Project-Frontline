using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public abstract class Commander : MonoBehaviour, IFactionComponent
{
    public string Name;
    public int Credits;
    public Faction Faction;

    public bool IsEliminated => Fortress == null;
    public Transform Fortress;
    private bool _ded = false;

    public GameObject[] AvailableUnits;

    public event Action<Commander> OnEliminated;

    protected virtual void Awake()
    {
        gameObject.name = Name;
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

    public void AssignCommander(GameObject obj)
    {
        var components = obj.GetComponentsInChildren<ICommanderComponent>();
        foreach (var component in components)
        {
            component.AssignCommander(this);
        }
    }

    public void SetFaction(Faction faction)
    {
        Faction = faction;
    }
}
