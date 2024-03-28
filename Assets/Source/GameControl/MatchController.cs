using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    private static MatchController _instance;
    private static float _matchStartTime;

    public static float MatchStartTime => _matchStartTime;
    public static float MatchTime => Time.time - _matchStartTime;

    public bool InitializeCurrentSettingsOnAwake;

    public event Action<MatchController, MatchSetup> OnMatchStarted;
    public event Action<MatchController, MatchSetup, MatchResult> OnMatchEnded;

    public MatchSetup MatchSettings { get; private set; }
    private VictoryChecker _victoryChecker;

    public PlayerStatTracker StatTracker;

    public bool MatchCompleted { get; private set; }
    public MatchResult MatchResult { get; private set; }

    public void Start ()
    {
        if (InitializeCurrentSettingsOnAwake)
        {
            StartMatch(MatchSetup.Current);
        }
    }

    public void StartMatch(MatchSetup settings)
    {
        Debug.Log("Match controller: Match started!");

        MatchSettings = settings;
        MatchResult = new MatchResult();
        MatchInitializer.GetInstance().InitializeMatch(MatchSettings);

        Commander[] commanders = Team.AllCommanders.ToArray();
        StatTracker.BeginTracking(commanders);

        StartCoroutine(PostInit());
        _victoryChecker = settings.VictoryChecker;
        _victoryChecker.OnVictor += OnVictor;

        _matchStartTime = Time.time;
        OnMatchStarted?.Invoke(this, MatchSettings);
    }

    private IEnumerator PostInit ()
    {
        yield return null;
        _victoryChecker.Initialize();
    }

    public static Commander GetCommanderFor(PlayerInfo playerInfo)
        => Team.AllCommanders.FirstOrDefault(x => x.PlayerInfo.Id == playerInfo.Id);

    private void OnVictor(Team obj)
    {
        Debug.Log("Match controller: Match completed!");

        MatchResult.WinningTeam = obj.TeamInfo;
        MatchResult.Winners = MatchSettings.Players.Where(x => x.Team == obj.TeamInfo).ToArray();
        MatchResult.WinnerStats = StatTracker.GetStatsFor(MatchResult.Winners);

        MatchCompleted = true;
        StatTracker.EndTracking(Team.AllCommanders.ToArray());

        OnMatchEnded?.Invoke(this, MatchSettings, MatchResult);
        Debug.Log("Match controller: Match ended!");
    }

    public static MatchController GetInstance()
    {
        if (_instance == null)
            _instance = FindAnyObjectByType<MatchController>();
        return _instance;
    }
}
