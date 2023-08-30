using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoGarrisonNearby : MonoBehaviour, ITeamComponent
{
    public float MaxRange;
    public InfantryGarrison Garrison;
    public bool Repeat;
    public bool PeriodicallyEjectGarrison;

    public enum Prioritization { Distance, DPS, Cost }
    public Prioritization PrioritizationFunction = Prioritization.DPS;

    private Dictionary<Prioritization, Func<Collider, Transform, float>> _prioritizationFunctions = new Dictionary<Prioritization, Func<Collider, Transform, float>>()
    {
        { Prioritization.Distance, (x, y) => Vector3.SqrMagnitude(x.transform.position - y.position) },
        { Prioritization.DPS, (x, y) => -1 * x.GetComponentInParent<Unit>().GetWeapons().Sum(y => y.GetDPSOrOverride()) },
        { Prioritization.Cost, (x, y) => -1 * x.GetComponentInParent<Unit>().Info.Cost },
    };

    private TeamInfo _team;

    public void SetTeam(TeamInfo team)
    {
        _team = team;
        if (Repeat)
        {
            InvokeRepeating(nameof(Fill), 1, 1);
        }
        else
        {
            Invoke(nameof(Fill), 1);
        }

        if (PeriodicallyEjectGarrison)
        {
            InvokeRepeating(nameof(Eject), 20, 20);
        }
    }

    private void Eject()
    {
        Garrison.EvacuateAll();
    }

    private void Fill ()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, MaxRange, _team.GetLayerMask())
            .Where(x => Garrison.CanGarrison(x.transform.root.gameObject))
            .GroupBy(x => x.transform.root).Select(x => x.First()).ToArray();
        float[] keys = hit.Select(x => _prioritizationFunctions[PrioritizationFunction](x, transform)).ToArray();

        Array.Sort(keys, hit);

        int toFill = Mathf.Min(hit.Length, Garrison.AvailableCount);
        for (int i = 0; i < toFill; i++)
        {
            Garrison.EnterGarrison(hit[i].transform.root.gameObject);
        }
    }
}
