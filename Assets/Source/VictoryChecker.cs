using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VictoryChecker : ScriptableObject
{
    public string Name;
    public string Description;

    public event Action<Team> OnVictor;

    private Team _victor;

    protected void SetVictor(Team team)
    {
        _victor = team;
        OnVictor?.Invoke(team);
    }

    public bool GameOver() 
        => GetVictor() != null;

    public Team GetVictor()
        => _victor;

    public abstract void Initialize();

    public static VictoryChecker[] LoadAll()
        => Resources.LoadAll<VictoryChecker>("VictoryCheckers");
}
