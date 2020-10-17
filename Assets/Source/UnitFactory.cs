using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour, IFactionComponent
{
    public GameObject UnitPrefab;
    public float SpawnDelay;
    public float SpawnRange;

    private Faction _faction;
    private Waypoint _nearestWaypoint;

    public void Start()
    {
        InvokeRepeating("Spawn", SpawnDelay, SpawnDelay);
        _nearestWaypoint = Waypoint.GetNearest(transform.position);
    }

    private void Spawn ()
    {
        Vector3 pos = GetLocalRandomSpawnPosition() + transform.position;
        GameObject go = _faction.Instantiate(UnitPrefab, pos, transform.rotation);
        go.SendMessage("SetWaypoint", _nearestWaypoint);
    }

    private Vector3 GetLocalRandomSpawnPosition ()
    {
        Vector3 unitSphere = Random.insideUnitSphere * SpawnRange;
        return new Vector3(unitSphere.x, 0f, unitSphere.z);
    }

    public void SetFaction(Faction faction)
    {
        _faction = faction;
    }
}
