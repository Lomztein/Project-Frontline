using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableParticleCollisionForTeam : MonoBehaviour, ITeamComponent
{
    public ParticleSystem System;

    public void SetTeam(TeamInfo team)
    {
        ParticleSystem.CollisionModule module = System.collision;
        module.collidesWith = module.collidesWith ^ team.GetLayerMask();
    }
}
