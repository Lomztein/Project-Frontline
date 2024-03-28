using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchRunner : MonoBehaviour
{
    private const string RUNNER_RESOURCE_PATH = "GameControl/MatchRunner";
    
    private const string BATTLEFIELD_SCENE_NAME = "Battlefield";
    private bool _currentMatchRunning;

    public MatchResult LastMatchResult;

    private static MatchRunner _instance;

    public event Action<MatchSetup, MatchResult> OnMatchFinished;

    public static MatchRunner GetInstance()
    {
        if (_instance == null )
        {
            _instance = Instantiate(Resources.Load<GameObject>(RUNNER_RESOURCE_PATH)).GetComponent<MatchRunner>();
            DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
    }

    public void RunMatch(MatchSetup matchSetup)
    {
        StartCoroutine(RunMatchAsync(matchSetup));
    }

    public IEnumerator RunMatchAsync(MatchSetup matchSetup)
    {
        Debug.Log("Matchrunner: Loading battlefield scene.");
        SceneManager.LoadScene(BATTLEFIELD_SCENE_NAME);
        yield return null;

        MatchInitializer initializer = MatchInitializer.GetInstance();
        MatchController controller = MatchController.GetInstance();

        controller.InitializeCurrentSettingsOnAwake = false;
        controller.OnMatchEnded += Controller_OnMatchEnded;
        controller.OnMatchStarted += Controller_OnMatchStarted;

        Debug.Log("Matchrunner: Starting match.");
        controller.StartMatch(matchSetup);

        while (_currentMatchRunning)
        {
            yield return new WaitForSecondsRealtime(1);
        }

        controller.OnMatchEnded -= Controller_OnMatchEnded;
        controller.OnMatchStarted -= Controller_OnMatchStarted;

        OnMatchFinished?.Invoke(matchSetup, LastMatchResult);
    }

    private void Controller_OnMatchStarted(MatchController arg1, MatchSetup arg2)
    {
        _currentMatchRunning = true;
    }

    private void Controller_OnMatchEnded(MatchController arg1, MatchSetup arg2, MatchResult result)
    {
        Debug.Log("Matchrunner: Match ended.");
        _currentMatchRunning = false;
        LastMatchResult = result;
    }
}
