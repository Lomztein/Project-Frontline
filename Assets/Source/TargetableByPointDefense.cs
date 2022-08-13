using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableByPointDefense : MonoBehaviour, ITeamComponent
{
    public void SetTeam(TeamInfo team)
    {
        gameObject.layer = team.ProjectileGetLayer();
    }
}
