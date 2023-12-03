using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mutators/Health Multiplier")]
public class HealthMultiplierMutator : Mutator
{
    public float Multiplier;

    public override void Start()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander");
        foreach (var commanderObj in commanders)
        {
            var commander = commanderObj.GetComponent<Commander>();
            commander.OnUnitPlaced += Commander_OnUnitPlaced;
            commander.OnUnitSpawned += Commander_OnUnitSpawned;
        }
    }

    private void Commander_OnUnitSpawned(Commander arg1, UnitFactory arg2, Unit arg3)
    {
        MultiplyHealth(arg3);
    }

    private void Commander_OnUnitPlaced(Commander arg1, Unit arg2)
    {
        MultiplyHealth(arg2);
    }

    private void MultiplyHealth(Unit unit)
    {
        var healths = unit.GetComponentsInChildren<Health>();
        foreach (var health in healths)
        {
            float prev = health.MaxHealth;
            health.MaxHealth *= Multiplier;
            float gained = health.MaxHealth - prev;
            health.Heal(new DamageInfo(gained, DamageModifier.One, health.transform.position, Vector3.forward, this, health));
            // I'm sure there is a smarter way but I'm caveman brain.
        }
    }

    public override void Stop()
    {
        var commanders = GameObject.FindGameObjectsWithTag("Commander");
        foreach (var commanderObj in commanders)
        {
            var commander = commanderObj.GetComponent<Commander>();
            commander.OnUnitPlaced -= Commander_OnUnitPlaced;
            commander.OnUnitSpawned -= Commander_OnUnitSpawned;
        }
    }
}
