using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemiesEleminatedVictoryChecker", menuName = "Victory Checkers/Enemies Eliminated")]
public class EnemiesEleminatedVictoryChecker : VictoryChecker
{
    public override void Initialize()
    {
        var commanders = Team.AllTeams.SelectMany(x => x.GetCommanders());
        foreach (Commander commander in commanders) {
            commander.OnEliminated += Commander_OnEliminated;
        }
    }

    private void Commander_OnEliminated(Commander obj)
    {
        TryFindVictor();
    }

    private bool TryFindVictor()
    {
        List<Team> remainingTeams = Team.AllTeams.ToList();
        var aliveCommanders = Team.AllTeams.SelectMany(x => x.GetCommanders().Where(x => !x.Eliminated));

        foreach (var team in Team.AllTeams)
        {
            if (!aliveCommanders.Any(x => x.TeamInfo == team.TeamInfo))
                remainingTeams.Remove(team);
        }

        if (remainingTeams.Count == 1)
        {
            SetVictor(remainingTeams.First());
        }

        throw new InvalidOperationException("Somehow, there are no teams alive.");
    }
}
