using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchScheduleTest : MonoBehaviour {

    private static bool IsRunning;
    public MatchSetup[] MatchesToRun;

    public void Start()
    {
        if (IsRunning == false)
        {
            DontDestroyOnLoad(gameObject);
            IsRunning = true;

            var matchesToRun = new List<MatchSetup>();
            var factions = Faction.LoadFactions().ToArray();
            var ais = AIPlayerProfile.LoadAll().ToArray();

            foreach (var faction1 in factions)
            {
                foreach (var faction2 in factions)
                {
                    MatchSetup settings = Instantiate(MatchSetup.Default());
                    settings.Players[0].Faction = faction1;
                    settings.Players[1].Faction = faction2;

                    matchesToRun.Add(settings);
                }
            }

            MatchesToRun = matchesToRun.ToArray();
            StartCoroutine(RunMatches(MatchesToRun));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RunMatches(IEnumerable<MatchSetup> matchesToRun)
    {
        foreach (var match in matchesToRun)
        {
            yield return MatchRunner.GetInstance().RunMatchAsync(match);
        }
    }
}
