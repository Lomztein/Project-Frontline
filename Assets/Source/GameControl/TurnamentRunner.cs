using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class TurnamentRunner : MonoBehaviour
{
    public const string CONTROLLER_RESOURCE_PATH = "GameControl/TurnamentRunner";
    public const string TURNAMENT_INTERMISSION_SCENE_NAME = "TurnamentIntermission";

    public enum TurnamentState { Setup, Match, Intermission }
    public enum TurnamentType { SingleElimination }

    public TurnamentState State;
    public TurnamentType Type;

    private static TurnamentRunner _instance;

    public PlayerInfo[] PlayerInfos { get; private set; }
    public TurnamentPlayerNode[] PlayerNodes { get; private set; }
    public TurnamentMatchNode[] FightNodes { get; private set; }
    public TurnamentPlayerNode VictorNode { get; private set; }

    public TurnamentMatchNode CurrentMatchNode { get; private set; }

    private bool _running;

    public static TurnamentRunner GetInstance()
    {
        if (_instance == null)
        {
            _instance = Instantiate(Resources.Load<GameObject>(CONTROLLER_RESOURCE_PATH)).GetComponent<TurnamentRunner>();
            DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
    }

    public void StartTurnament(IEnumerable<PlayerInfo> players)
    {
        PlayerInfos = players.ToArray();
        SetupNodes(players);
        StartCoroutine(Run());
    }

    private IEnumerator Run ()
    {
        _running = true;
        while(_running)
        {
            yield return Intermission();
            yield return Match();
            HandleMatchResult();
        }
        if (IsVictorFound())
        {
            yield return Victory(VictorNode);
        }
    }

    private IEnumerator Intermission()
    {
        State = TurnamentState.Intermission;
        Debug.Log("Turnament controller: Intermission");
        SceneManager.LoadScene(TURNAMENT_INTERMISSION_SCENE_NAME);
        yield return new WaitForSecondsRealtime(5);
    }

    private IEnumerator Match ()
    {
        Debug.Log("Turnament controller: Match");
        State = TurnamentState.Match;
        CurrentMatchNode = FindNextMatch();
        if (CurrentMatchNode != null)
        {
            yield return MatchRunner.GetInstance().RunMatchAsync(SetupMatch(CurrentMatchNode));
        }
        Debug.Log("Turnament controller: Match completed");
    }

    private void HandleMatchResult()
    {
        Debug.Log("Turnament controller: Handle match result");
        MatchResult matchResult = MatchRunner.GetInstance().LastMatchResult;
        CurrentMatchNode.SetWinnerNodePlayer(matchResult.Winners[0]); 
    }

    private bool IsVictorFound()
        => VictorNode.PlayerInfo != null;

    private IEnumerator Victory(TurnamentPlayerNode victorNode)
    {
        Debug.Log($"Turnament controller: Victor {victorNode}");
        yield return Intermission();
    }

    private TurnamentMatchNode FindNextMatch ()
    {
        return FightNodes.FirstOrDefault(x => x.IsReady);
    }

    private MatchSetup SetupMatch(TurnamentMatchNode matchNode)
    {
        MatchSetup setup = MatchSetup.Default();
        setup.ClearPlayers();
        foreach (var player in matchNode.PlayerNodes)
        {
            setup.AddPlayer(player.PlayerInfo);
        }

        for (int i = 0; i < setup.Players.Length; i++)
        {
            PlayerInfo playerInfo = setup.Players[i];
            playerInfo.SpawnIndex = i;
            playerInfo.Team = TeamInfo.LoadTeams()[i];
            playerInfo.Id = (uint)i;
        }
        return setup;
    }

    private void SetupNodes(IEnumerable<PlayerInfo> players)
    {
        if (Type == TurnamentType.SingleElimination)
        {
            SetupSingleEliminationNodes(players);
        }
    }

    private void SetupSingleEliminationNodes(IEnumerable<PlayerInfo> players)
    {
        List<TurnamentPlayerNode> playerNodes = new List<TurnamentPlayerNode>();
        List<TurnamentMatchNode> matchNodes = new List<TurnamentMatchNode>();

        PlayerInfo[] arr = players.ToArray();
        int numPlayers = arr.Length;
        int closestPowerOfTwo = Mathf.NextPowerOfTwo(numPlayers);
        int sets = Mathf.RoundToInt(Mathf.Log(closestPowerOfTwo, 2f)) + 1;

        for (int i = 0; i < sets; i++)
        {
            int numSlots = closestPowerOfTwo / (int)Mathf.Pow(2, i);
            playerNodes.AddRange(Enumerable.Range(0, numSlots).Select(x => new TurnamentPlayerNode()));
        }

        for (int i = 0; i < numPlayers; i++)
        {
            playerNodes[i].PlayerInfo = arr[i];
        }

        int startIndex = 0;
        for (int i = 0; i < sets - 1; i++)
        {
            ConnectSetToNext(startIndex, i, numPlayers, playerNodes, matchNodes);
            startIndex += closestPowerOfTwo / (int)Mathf.Pow(2, i);
        }

        // Collapse match nodes with only one player
        foreach (var matchNode in matchNodes)
        {
            if (matchNode.PlayerNodes.Count(x => x.PlayerInfo != null) == 1)
            {
                matchNode.SetWinnerNodePlayer(matchNode.PlayerNodes[0].PlayerInfo);
            }
        }

        PlayerNodes = playerNodes.ToArray();
        FightNodes = matchNodes.ToArray();

        VictorNode = playerNodes.Last();
    }

    private void ConnectSetToNext(int startIndex, int setIndex, int numPlayers, List<TurnamentPlayerNode> playerNodes, List<TurnamentMatchNode> matchNodes)
    {
        int numSlots = Mathf.NextPowerOfTwo(numPlayers) / (int)Mathf.Pow(2, setIndex);
        for (int i = 0; i < numSlots; i += 2)
        {
            int playerNodeOneIndex = startIndex + i;
            int playerNodeTwoIndex = startIndex + i + 1;
            int victorPlayerNodeIndex = startIndex + numSlots + Mathf.FloorToInt(i / 2f) ;

            TurnamentMatchNode matchNode = new TurnamentMatchNode
            {
                PlayerNodes = new TurnamentPlayerNode[2]
            };
            matchNode.PlayerNodes[0] = playerNodes[playerNodeOneIndex];
            matchNode.PlayerNodes[1] = playerNodes[playerNodeTwoIndex];
            matchNode.WinnerNode = playerNodes[victorPlayerNodeIndex];

            matchNodes.Add(matchNode);
        }
    }

    public void Cancel ()
    {
        _running = false;
    }
}
