﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamApplier : MonoBehaviour
{
    public TeamInfo Team;
    public void Awake()
    {
        Team.ApplyTeam(gameObject);
    }

}
