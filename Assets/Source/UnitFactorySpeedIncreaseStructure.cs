using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactorySpeedIncreaseStructure : MonoBehaviour, ITeamComponent
{
    private TeamInfo _team;

    public float Range;
    public float SpeedMulitplier;

    private List<UnitFactory> _affectedFactories = new List<UnitFactory>();

    private void Start()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, Range, _team.GetLayerMask());
        foreach (var col in nearby)
        {
            UnitFactory factory = col.GetComponent<UnitFactory>();
            if (factory)
            {
                factory.SpawnDelay /= SpeedMulitplier + 1;
                _affectedFactories.Add(factory);
            }
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            foreach (var factory in _affectedFactories)
            {
                if (factory)
                {
                    factory.SpawnDelay *= SpeedMulitplier + 1;
                }
            }
        }
    }

    public void SetTeam(TeamInfo team)
    {
        _team = team;
    }
}
