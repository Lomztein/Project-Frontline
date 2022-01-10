using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour, ITeamComponent
{
    public GameObject UnitPrefab;
    public float SpawnDelay;
    public float SpawnRange;

    private TeamInfo _team;
    private Waypoint _nearestWaypoint;

    public event Action<UnitFactory, GameObject> OnUnitSpawned;

    public void Start()
    {
        InvokeRepeating("Spawn", SpawnDelay, SpawnDelay);
        _nearestWaypoint = Waypoint.GetNearest(transform.position);
    }

    private void Spawn ()
    {
        Vector3 pos = GetLocalRandomSpawnPosition() + transform.position;
        GameObject go = _team.Instantiate(UnitPrefab, pos, transform.rotation);
        go.BroadcastMessage("SetWaypoint", _nearestWaypoint);
        OnUnitSpawned?.Invoke(this, go);
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
}
