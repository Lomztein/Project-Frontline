using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class MatchInitializer : MonoBehaviour
{
    public bool InitializeCurrentSettingsOnAwake;

    public GameObject TeamPrefab;
    public GameObject AIPrefab;
    public GameObject PlayerPrefab;

    private void Start()
    {
        if (InitializeCurrentSettingsOnAwake)
        {
            InitializeMatch(MatchSettings.Current);
        }
    }

    public void InitializeMatch (MatchSettings settings)
    {
        settings.MapInfo.SceneryGenerator.Generate(settings.MapInfo);
        settings.MapInfo.Shape.GenerateNodes(settings.MapInfo).ToArray();

        var teams = settings.Players.Select(x => x.Team).ToArray();
        var teamObjects = new Team[teams.Length];

        var spawnLines = settings.MapInfo.Shape.GenerateSpawnVolumes(settings.MapInfo).ToArray();

        for (int i = 0; i < teams.Length; i++)
        {
            Team team = SpawnTeam(teams[i], Vector3.zero, Quaternion.identity);
            teamObjects[i] = team;
        }

        if (!settings.Players.Any(x => x.IsPlayer))
        {
            var defaultPlayer = new MatchSettings.PlayerInfo();
            defaultPlayer.PlayerInputType = PlayerHandler.InputType.MouseAndKeyboard;
            defaultPlayer.PlayerInputDeviceId = 0;
            defaultPlayer.Id = uint.MaxValue - 1;
            settings.AddPlayer(defaultPlayer);
        }

        var players = settings.Players.ToArray();
        List<Commander> commanders = new List<Commander>();
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].IsObserver)
            {
                int spawn = Mathf.Min(players[i].SpawnIndex, spawnLines.Length - 1);
                int team = Array.IndexOf(teams, players[i].Team);
                var cospawns = players.Where(x => x.SpawnIndex == spawn).ToArray();
                int spawnIndex = Array.IndexOf(cospawns, players[i]);

                Vector3 position = spawnLines[spawn].Position;
                Quaternion rotation = spawnLines[spawn].Rotation;

                Vector3 playerPosition = spawnLines[spawn].GetSpawnPoint(spawnIndex, cospawns.Length);
                Commander com = SpawnCommander(players[i], playerPosition, rotation);
                com.transform.SetParent(transform, true);
                commanders.Add(com);
            }
        }

        InitializePlayers(settings, commanders);

        for (int i = 0; i < teams.Length; i++)
        {
            teams[i].ApplyTeam(teamObjects[i].gameObject);
        }


        foreach (var mutator in settings.Mutators)
        {
            mutator.Start();
        }

        GameObject.Find("Sun").GetComponent<DayNightCycle>().Behaviour = settings.DayNightBehaviour;
        StartCoroutine(PostInit());
    }

    public void InitializePlayers (MatchSettings settings, IEnumerable<Commander> commanders)
    {
        var players = settings.Players.Where(x => x.IsPlayer).ToList();
        var viewPorts = CameraUtils.ComputeViewportRects(players.Count).ToArray();
        var playerCommanders = players.Select(x => commanders.FirstOrDefault(y => y.OwnerId == x.Id)).ToArray();
        var playerPositions = playerCommanders.Select(x => x == null ? Vector3.zero : x.Fortress.position).Select(x => new Vector3(-x.z, x.y)).ToArray();
        viewPorts = CameraUtils.MatchPositions(viewPorts, playerPositions);

        PlayerHandler[] handlers = new PlayerHandler[players.Count];
        handlers[0] = GameObject.Find("DefaultPlayerHandler").GetComponent<PlayerHandler>();
        for (int i = 1; i < players.Count; i++)
        {
            handlers[i] = PlayerHandler.CreateNewPlayer();
        }

        for (int i = 0; i < handlers.Length; i++)
        {
            handlers[i].Assign(playerCommanders[i], players[i].PlayerInputType, players[i].PlayerInputDeviceId, viewPorts[i]);
        }
    }

    private Team SpawnTeam (TeamInfo teamInfo, Vector3 position, Quaternion rotation)
    {
        GameObject teamObj = Instantiate(TeamPrefab, position, rotation);
        Team team = teamObj.GetComponent<Team>();
        team.TeamInfo = teamInfo;
        return team;
    }

    private Commander SpawnCommander(MatchSettings.PlayerInfo info, Vector3 position, Quaternion rotation)
    {
        GameObject commanderObj;
        if (info.AIProfile == null)
        {
            commanderObj = Instantiate(PlayerPrefab, position, rotation);
        }
        else
        {
            commanderObj = Instantiate(AIPrefab, position, rotation);
            AICommander aiCom = commanderObj.GetComponent<AICommander>();
            aiCom.TargetAvarageAPM = info.AIProfile.ActionsPerMinute;
            var selector = aiCom.GetComponent<WeightedUnitSelector>();
            selector.WeightTable = info.AIProfile.UnitWeightTable;
            aiCom.SaveTimeMinMax = info.AIProfile.SaveTimeMinMax;
            aiCom.SaveTimeBias = info.AIProfile.SaveTimeBias;
        }
        Commander commander = commanderObj.GetComponent<Commander>();
        commander.OwnerId = info.Id;
        commander.Fortress = Instantiate(info.Faction.HeadquartersPrefab, commanderObj.transform).transform;
        commander.Credits = info.StartingCredits;
        commander.Name = info.Name
            .Replace("{Faction}", info.Faction == null ? "Observer" : info.Faction.Name)
            .Replace("{Type}", info.AIProfile == null ? "No AI" : info.AIProfile.Name);
        commander.UnitAvailable = info.UnitAvailable;
        commander.Faction = info.Faction;

        info.Team.ApplyTeam(commanderObj);
        commander.AssignCommander(commanderObj);

        return commander;
    }

    IEnumerator PostInit ()
    {
        yield return new WaitForFixedUpdate();
        MatchSettings.Current.ProductionBehaviour.OnMatchInitialized();
    }

    private void OnDrawGizmos()
    {
        var parimiter = MatchSettings.Current.MapInfo.GetPerimeterPolygon().ToArray();
        for (int i = 0; i < parimiter.Length - 1; i++)
        {
            Gizmos.DrawLine(parimiter[i] + Vector3.up, parimiter[i + 1] + Vector3.up);
        }
    }
}
