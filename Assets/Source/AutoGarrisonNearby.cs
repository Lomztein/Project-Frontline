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
    }

        private void Fill ()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, MaxRange, _team.GetLayerMask())
            .Where(x => Garrison.CanGarrison(x.transform.root.gameObject))
            .GroupBy(x => x.transform.root).Select(x => x.First()).ToArray();
        float[] keys = hit.Select(x => Vector3.SqrMagnitude(x.transform.position - transform.position)).ToArray();
        Array.Sort(keys, hit);

        int toFill = Mathf.Min(hit.Length, Garrison.AvailableCount);
        for (int i = 0; i < toFill; i++)
        {
            Garrison.EnterGarrison(hit[i].transform.root.gameObject);
        }
    }
}
