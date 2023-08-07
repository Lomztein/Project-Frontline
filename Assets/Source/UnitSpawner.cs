using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour, ICommanderComponent, ITeamComponent
{
    public bool SpawnOnStart;
    public float SpawnsPerSecond;

    public GameObject[] UnitPrefabs;
    public enum SpawnBehaviour { Random, RoundRobin }
    public SpawnBehaviour UnitSpawnBehaviour;
    private int _prefabIndex = -1;

    public Transform[] SpawnPositions;
    public bool ParentSpawns;
    public float SpawnRange;
    public enum PositionBehaviour { Random, All, RoundRobin }
    public PositionBehaviour UnitPositionBehaviour;
    private int _positionIndex = 0;

    private Commander _commander;
    private TeamInfo _teamInfo;

    public Action<Unit> OnUnitSpawned;

    private void Start()
    {
        if (SpawnOnStart)
        {
            Spawn();
        }

        if (SpawnsPerSecond > 0.01)
        {
            InvokeRepeating(nameof(Spawn), 1f / SpawnsPerSecond, 1f / SpawnsPerSecond);
        }
    }

    public void AssignCommander(Commander commander)
    {
        _commander = commander;
    }

    public void SetTeam(TeamInfo team)
    {
        _teamInfo = team;
    }

    private GameObject GetUnitToSpawn ()
    {
        if (UnitSpawnBehaviour == SpawnBehaviour.Random)
        {
            return UnitPrefabs[UnityEngine.Random.Range(0, UnitPrefabs.Length)];
        }else if (UnitSpawnBehaviour == SpawnBehaviour.RoundRobin)
        {
            return UnitPrefabs[++_prefabIndex % UnitPrefabs.Length]; // I don't know if you can do that so might cause a bug lol
        }
        throw new InvalidOperationException("Invalid unit spawn behaviour selected.");
    }

    private void Spawn ()
    {
        if (UnitPositionBehaviour == PositionBehaviour.All)
        {
            foreach (var position in SpawnPositions)
            {
                GameObject prefab = GetUnitToSpawn();
                SpawnUnit(prefab, position);
            }
        }else
        {

            Transform position = null;
            if (UnitPositionBehaviour == PositionBehaviour.Random)
            {
                position = SpawnPositions[UnityEngine.Random.Range(0, SpawnPositions.Length)];
            }else if (UnitPositionBehaviour == PositionBehaviour.RoundRobin) {
                position = SpawnPositions[++_positionIndex % SpawnPositions.Length]; // I don't know if you can do that so might cause a bug lol
            }

            GameObject prefab = GetUnitToSpawn();
            SpawnUnit(prefab, position);
        }
    }

    private void SpawnUnit (GameObject unit, Transform position)
    {
        GameObject newUnit = Instantiate(unit, position);
        if (!ParentSpawns) newUnit.transform.SetParent(null);
        if (_teamInfo != null) _teamInfo.ApplyTeam(newUnit);
        if (_commander != null) _commander.AssignCommander(newUnit);
    }
}
