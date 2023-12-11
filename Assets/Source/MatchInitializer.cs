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
        var players = settings.Players.ToArray();

        var spawnLines = settings.MapInfo.Shape.GenerateSpawnVolumes(settings.MapInfo).ToArray();

        for (int i = 0; i < teams.Length; i++)
        {
            Team team = SpawnTeam(teams[i], Vector3.zero, Quaternion.identity);
            teamObjects[i] = team;
        }

        for (int i = 0; i < players.Length; i++)
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
        }

        for (int i = 0; i < teams.Length; i++)
        {
            teams[i].ApplyTeam(teamObjects[i].gameObject);
        }


        foreach (var mutator in settings.Mutators)
        {
            mutator.Start();
        }

        StartCoroutine(PostInit());
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
        if (info.IsPlayer)
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
        commander.Fortress = Instantiate(info.Faction.HeadquartersPrefab, commanderObj.transform).transform;
        commander.Credits = info.StartingCredits;
        commander.Name = info.Name;
        commander.UnitAvailable = info.UnitAvailable;
        commander.Faction = info.Faction;

        info.Team.ApplyTeam(commanderObj);
        commander.AssignCommander(commanderObj);

        if (info.IsPlayer)
        {
            AttachPlayerToPurchaseMenu(commander);
            MatchController.SetPlayerCommander(commander);
            Camera.main.GetComponent<TopDownCameraController>().MoveToHQ();
        }
        return commander;
    }

    IEnumerator PostInit ()
    {
        yield return new WaitForFixedUpdate();
        MatchSettings.Current.ProductionBehaviour.OnMatchInitialized();
    }

    private void AttachPlayerToPurchaseMenu (Commander player)
    {
        UnitPurchaseMenu menu = GameObject.Find("PurchaseMenu").GetComponent<UnitPurchaseMenu>();
        menu.Commander = player;
        menu.UpdateActive();
        FindObjectOfType<CreditsDisplay>().Commander = player;
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
