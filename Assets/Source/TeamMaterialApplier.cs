using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMaterialApplier : MonoBehaviour, ITeamComponent
{
    [SerializeField] private Renderer[] _renderersToApplyTo;

    public void SetTeam(TeamInfo team)
    {
        foreach (Renderer renderer in _renderersToApplyTo)
        {
            renderer.material = team.TeamMaterial;
        }
    }
}
