using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour, IFactionComponent
{
    public GameObject UnitPrefab;
    public float SpawnDelay;
    public float SpawnRange;

    private Faction _faction;

    public void Start()
    {
        InvokeRepeating("Spawn", SpawnDelay, SpawnDelay);
    }

    private void Spawn ()
    {
        Vector3 pos = GetLocalRandomSpawnPosition() + transform.position;
        _faction.Instantiate(UnitPrefab, pos, transform.rotation);
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
