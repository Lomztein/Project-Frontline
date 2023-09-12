using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Team : MonoBehaviour, ITeamComponent
{
    private static List<Team> _teams = new List<Team>();

    public TeamInfo TeamInfo;
    private List<Commander> _commanders = new List<Commander>();

    private void Awake()
    {
        _teams.Add(this);
    }

    private void OnDestroy()
    {
        _teams.Remove(this);
    }

    public void SetTeam(TeamInfo team)
    {
        TeamInfo = team;
    }

    public static Team GetTeam(TeamInfo info) => _teams.Find(x => x.TeamInfo == info);
    public static Team FindTeam(Func<Team, bool> predicate) => _teams.FirstOrDefault(predicate);

    public static IEnumerable<Team> GetOtherTeams(TeamInfo info) => _teams.FindAll(x => x.TeamInfo != info);

    public void AddCommander(Commander commander) => _commanders.Add(commander);
    public void RemoveCommander(Commander commander) => _commanders.Remove(commander);
    public Commander[] GetCommanders() => _commanders.ToArray();
}
