using CustomGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitFactory : MonoBehaviour, ITeamComponent, ICommanderComponent
{
    public GameObject UnitPrefab;
    private Unit _unit;

    public float SpawnDelay;
    public float SpawnRange;

    private TeamInfo _team;
    private NavigationNode _nearestWaypoint;
    private Commander _commander;

    public float EarnedCredits; // The amount of money the units from this factory has earned.
    public float GivenCredits; // The amount of money the units from this factory has given by being killed by enemies.
    public float Effeciency => EarnedCredits / GivenCredits;

    public event Action<UnitFactory, GameObject> OnUnitSpawned;
    private UnitProductionBehaviour.UnitProductionCallback _callback;

    public float NextProductionTime => _callback.NextProductionTime;
    public float ProductionTime => _callback.ProductionTime;

    public void Start()
    {
        _callback = MatchSetup.GetCurrent().ProductionBehaviour.CreateCallback();
        _callback.Initialize(_commander, SpawnDelay, Spawn);

        _unit = UnitPrefab.GetComponent<Unit>();
        _nearestWaypoint = Navigation.GetNearestNode(transform.position);
        EarnedCredits -= _unit.Info.Cost;
        GetComponent<Unit>().AddStat("Effeciency", Effeciency);
        GetComponent<Unit>().AddStat("Units spawned", 0);
        GetComponent<Unit>().AddStat("Kills", 0);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && _callback != null)
        {
            _callback.Stop();
        }
    }

    private void Spawn ()
    {
        Vector3 pos = GetLocalRandomSpawnPosition() + transform.position;
        GameObject go = _team.Instantiate(UnitPrefab, pos, transform.rotation);
        NavigationNode targetNode;
        if (_commander.Target)
        {
            targetNode = Navigation.GetNearestNode(_commander.Target.transform.position);
        }
        else
        {
            targetNode = Navigation.GetNearestNode(_commander.transform.position + _commander.transform.forward * 1000);
        }
        go.BroadcastMessage("SetPath", Navigation.GetPath(_nearestWaypoint, targetNode).ToArray(), SendMessageOptions.DontRequireReceiver);

        OnUnitSpawned?.Invoke(this, go);
        go.GetComponent<Health>().OnDeath += Unit_OnDeath;
        go.GetComponent<Unit>().OnKill += Unit_OnKill;
        GetComponent<Unit>().ChangeStat("Units spawned", 1);
    }

    private void Unit_OnKill(Unit arg1, IWeapon arg2, Projectile arg3, IDamagable arg4)
    {
        if (this != null && TryGetComponent(out Unit unit))
        {
            if (arg4 is Health health && health.TryGetComponent(out Unit killedUnit))
            {
                EarnedCredits += killedUnit.Info.Value;
                unit.SetStat("Effeciency", Effeciency);
                unit.ChangeStat("Kills", 1);
            }
        }
    }

    private void Unit_OnDeath(Health obj)
    {
        if (this != null && TryGetComponent(out Unit unit))
        {
            GivenCredits += obj.GetComponent<Unit>().Info.Value;
            obj.OnDeath -= Unit_OnDeath;
            unit.OnKill -= Unit_OnKill;
            GetComponent<Unit>().SetStat("Effeciency", Effeciency);
        }
    }

    private Vector3 GetLocalRandomSpawnPosition ()
    {
        Vector3 unitSphere = UnityEngine.Random.insideUnitSphere * SpawnRange;
        return new Vector3(unitSphere.x, 0f, unitSphere.z);
    }

    public void SetTeam(TeamInfo faction)
    {
        _team = faction;
    }

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }
}
